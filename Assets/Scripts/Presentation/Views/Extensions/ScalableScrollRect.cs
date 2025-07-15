using System;
using System.Collections.Generic;
using Presentation.Presenter;
using Presentation.Utilities;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLinq;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace Presentation.Views.Extensions
{
    public class ScalableScrollRect : ScrollRect
    {
        [SerializeField, Range(1f, 10f)] protected float verticalScaleMax = 4f;
        [SerializeField] private RectTransform verticalFollowContent, horizontalFollowContent;
        
#if UNITY_EDITOR
        [CustomEditor(typeof(ScalableScrollRect))]
        [CanEditMultipleObjects]
        public class ScalableScrollRectEditor : ScrollRectEditor
        {
            private SerializedProperty _scaleMaxProp;
            private SerializedProperty _vFollowContentProp, _hFollowContentProp;
            protected override void OnEnable()
            {
                _scaleMaxProp = serializedObject.FindProperty("verticalScaleMax");
                _vFollowContentProp = serializedObject.FindProperty("verticalFollowContent");
                _hFollowContentProp = serializedObject.FindProperty("horizontalFollowContent");
                base.OnEnable();
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                EditorGUILayout.LabelField("カーソルの中心を指すオブジェクト。", EditorStyles.label);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_scaleMaxProp);
                EditorGUILayout.PropertyField(_vFollowContentProp);
                EditorGUILayout.PropertyField(_hFollowContentProp);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    // any reloading
                }
                EditorGUILayout.Separator();
                
                base.OnInspectorGUI();
            }
        }
#endif
        
        private readonly Dictionary<int, int> _pointerToTouch = new(), _touchToPointer = new();
        private readonly ReactiveProperty<int> _pointersCount = new(0);

        // vertical settings
        private float _currentScale, _standardHeight;
        // horizontal settings
        private Action<int> _onPageSteppedCallback;
        private float _prevOffsetX, _thresholdRatio = 1.0f, _unitPageWidth = float.MaxValue;
        private bool _horizontalMoved;
        
        private int _scrollingPointerId = -1;
        
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            // すでに登録されているポインターIDは蹴る
            if (_pointerToTouch.ContainsKey(eventData.pointerId)) return;
            
            // 入力が検知されたPointerに最も近いまだ登録されていないTouchを、相互参照登録する
            var bestTouchId = -1;
            var bestDist = float.MaxValue;
            foreach (var t in Touch.activeTouches)
            {
                if (_touchToPointer.ContainsKey(t.touchId)) continue;
                // positionはどちらもスクリーン座標系らしい、ChatGPTによると
                var d = Vector2.SqrMagnitude(t.screenPosition - eventData.position);
                if (d >= bestDist) continue;
                bestDist = d;
                bestTouchId = t.touchId;
            }

            if (bestTouchId >= 0)
            {
                _pointerToTouch[eventData.pointerId] = bestTouchId;
                _touchToPointer[bestTouchId] = eventData.pointerId;
                _pointersCount.Value = _pointerToTouch.Count;
                
                // これが一つ目のポインターなら、移動方向を見て確定する
                if (_pointersCount.Value is 1)
                {
                    var v = Touch.activeTouches[0].delta;
                    var xAdv = Mathf.Abs(v.x) > Mathf.Abs(v.y);
                    horizontal = xAdv;
                    vertical = !xAdv;
                }
            }

            if (_pointersCount.Value == 1)
            {
                _scrollingPointerId = eventData.pointerId;
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_scrollingPointerId == eventData.pointerId) base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            // 手を離したタイミングで pointerId と touchId の連携を解く
            _touchToPointer.Remove(_pointerToTouch[eventData.pointerId]);
            _pointerToTouch.Remove(eventData.pointerId);
            _pointersCount.Value = _pointerToTouch.Count;
            if (_scrollingPointerId == eventData.pointerId)
            {
                _scrollingPointerId = -1;
                base.OnEndDrag(eventData);
            }

            // ポインターが全部離れたら、移動方向制限を解除する
            if (_pointersCount.Value is 0)
            {
                // 横方向に移動していた時、thresholdを超えてたらページをめくる
                if (horizontal)
                {
                    var pageStepped = 0;
                    while (Mathf.Abs(content.anchoredPosition.x) > _thresholdRatio * _unitPageWidth)
                    {
                        var stepDirection = -Mathf.Sign(content.anchoredPosition.x);
                        if (pageStepped is not 0 && (int)Mathf.Sign(pageStepped) != (int)stepDirection) break;
                        pageStepped += (int)stepDirection;
                        content.anchoredPosition += _unitPageWidth * stepDirection * Vector2.right;
                    }
                    _prevOffsetX = content.anchoredPosition.x;
                    if (pageStepped is not 0) _onPageSteppedCallback?.Invoke(pageStepped);
                }
                horizontal = true;
                vertical = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            _currentScale = 1f;
            _standardHeight = content.sizeDelta.y;
            
            _pointersCount.Subscribe(_ =>
            {
                _standardHeight = content.sizeDelta.y / _currentScale;
            }).AddTo(this);
        }

        private void Update()
        {
            if (!Application.isPlaying) return;

            if (vertical)
            {
                var touches = Touch.activeTouches.AsValueEnumerable()
                    .Where(t => _touchToPointer.ContainsKey(t.touchId));
                if (touches.Count() >= 2)
                {
                    var center = touches.Select(touch =>
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, touch.screenPosition,
                                InAppContext.SceneLoader.Camera, out var positionInRect)
                                ? positionInRect
                                : Vector2.zero)
                        .Average();
                    var absDiff = touches.Select(touch =>
                            ((RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, touch.screenPosition,
                                InAppContext.SceneLoader.Camera, out var currentInRect)
                                ? currentInRect
                                : Vector2.zero) - center).Abs()
                            - ((RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport,
                                touch.startScreenPosition,
                                InAppContext.SceneLoader.Camera, out var startInRect)
                                ? startInRect
                                : Vector2.zero) - center).Abs())
                        .Sum();

                    _currentScale = (content.sizeDelta + absDiff).y / content.sizeDelta.y;

                    content.sizeDelta =
                        content.sizeDelta = new Vector2(content.sizeDelta.x, _standardHeight * _currentScale);
                }
                else
                {
                    _currentScale = 1f;
                }
            }

            if (horizontal)
            {
                if (_pointersCount.Value is 0)
                {
                    if (_horizontalMoved)
                    {
                        velocity -= ReversingVelocity(content.anchoredPosition.x) * Vector2.right;
                        if ((int)Mathf.Sign(_prevOffsetX) != (int)Mathf.Sign(content.anchoredPosition.x) ||
                            Mathf.Abs(content.anchoredPosition.x) < 1f)
                        {
                            velocity = Vector2.zero;
                            content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
                            _horizontalMoved = false;
                        }
                    }
                    else
                    {
                        content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
                    }
                }
                else
                {
                    _horizontalMoved = true;
                }
                _prevOffsetX = content.anchoredPosition.x;
            }
            
            AdjustContent();
            return;

            float ReversingVelocity(float offset)
            {
                return (Mathf.Sqrt(Mathf.Abs(offset) + 1000) * 1000 * Mathf.Sign(offset)) * Time.deltaTime;
            }
        }

        public void SetStepPageSettings(float unitPageWidth, float thresholdPageStep, Action<int> onPageStepped)
        {
            _unitPageWidth = unitPageWidth;
            _thresholdRatio = thresholdPageStep;
            _onPageSteppedCallback = onPageStepped;
        }

        private void AdjustContent()
        {
            var suitableHeight =
                Mathf.Clamp(content.sizeDelta.y, viewport.rect.size.y, viewport.rect.size.y * verticalScaleMax);
            content.sizeDelta = new Vector2(content.sizeDelta.x, suitableHeight);
            
            verticalFollowContent.sizeDelta = new Vector2(verticalFollowContent.sizeDelta.x, suitableHeight);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            verticalFollowContent.anchoredPosition = new Vector2(verticalFollowContent.anchoredPosition.x, content.anchoredPosition.y);
            horizontalFollowContent.anchoredPosition = new Vector2(content.anchoredPosition.x, horizontalFollowContent.anchoredPosition.y);
        }
    }
}