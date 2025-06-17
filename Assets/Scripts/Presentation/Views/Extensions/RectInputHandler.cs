using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Presentation.Views.Extensions
{
    [RequireComponent(typeof(RectTransform), typeof(ObservableEventTrigger))]
    public class RectInputHandler : MonoBehaviour
    {
        public enum TouchState
        {
            None,
            Down,
            Move,
            Up
        }
        
        public ReactiveProperty<TouchState> State { get; } = new(TouchState.None);
        
        public Vector2 PositionOnRect { get; private set; }

        [SerializeField] private ObservableEventTrigger eventTrigger;
        
        private RectTransform _rt;
        private int? _activePointerId;
        private bool _justReleased;

        private void Awake()
        {
            _rt = (RectTransform)transform;

            // Down
            eventTrigger.OnPointerDownAsObservable()
                .Subscribe(evt =>
                {
                    // すでに他の指が入力中なら無視
                    if (_activePointerId != null) return;

                    _activePointerId = evt.pointerId;
                    HandleTouch(evt.position, evt.pressEventCamera, TouchState.Down);
                })
                .AddTo(eventTrigger);

            // Move (Drag)
            eventTrigger.OnDragAsObservable()
                .Subscribe(evt =>
                {
                    if (_activePointerId != evt.pointerId) return;
                    HandleTouch(evt.position, evt.pressEventCamera, TouchState.Move);
                })
                .AddTo(eventTrigger);

            // Up
            eventTrigger.OnPointerUpAsObservable()
                .Subscribe(evt =>
                {
                    if (_activePointerId != evt.pointerId) return;

                    HandleTouch(evt.position, evt.pressEventCamera, TouchState.Up);
                    _activePointerId = null;    // 解放
                })
                .AddTo(eventTrigger);
        }


        private void Update()
        {
            // グローバルに Up を検知（領域外で離しても拾う）
            if (_activePointerId != null)
            {
                var released = false;
                Vector2 pos = default;

                if (_activePointerId == PointerInputModule.kMouseLeftId)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        released = true;
                        pos = Input.mousePosition;
                    }
                }
                else
                {
                    foreach (var t in Input.touches)
                    {
                        if (t.fingerId == _activePointerId && t.phase == TouchPhase.Ended)
                        {
                            released = true;
                            pos = t.position;
                            break;
                        }
                    }
                }

                if (released)
                {
                    HandleTouch(pos, null, TouchState.Up);
                    _activePointerId = null;
                    _justReleased = true;
                }
            }

            // Up の次フレームで None に戻す
            if (_justReleased)
            {
                State.Value = TouchState.None;
                _justReleased = false;
            }
        }

        private void HandleTouch(Vector2 screenPos, Camera cam, TouchState state)
        {
            // ローカル座標に変換
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rt, screenPos, cam, out var localPos))
            {
                PositionOnRect = localPos;
            }
            State.Value = state;
        }
    }
}