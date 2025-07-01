using System;
using System.Collections.Generic;
using Presentation.Presenter;
using Presentation.Utilities;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
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
        [SerializeField, Range(1f, 10f)] protected float scaleMax = 4f;
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(ScalableScrollRect))]
        [CanEditMultipleObjects]
        public class ScalableScrollRectEditor : ScrollRectEditor
        {
            private SerializedProperty _scaleMaxProp;
            protected override void OnEnable()
            {
                _scaleMaxProp = serializedObject.FindProperty("scaleMax");
                base.OnEnable();
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                EditorGUILayout.LabelField("カーソルの中心を指すオブジェクト。", EditorStyles.label);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_scaleMaxProp);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    // todo any reloading
                }
                EditorGUILayout.Separator();
                
                base.OnInspectorGUI();
            }
        }
#endif
        
        private readonly Dictionary<int, int> _pointerToTouch = new(), _touchToPointer = new();
        private readonly ReactiveProperty<int> _pointersCount = new(0);

        private float _currentScale, _standardHeight;
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
            
            var touches = Touch.activeTouches.AsValueEnumerable().Where(t => _touchToPointer.ContainsKey(t.touchId));
            if (touches.Count() >= 2)
            {
                var center = touches.Select(touch => 
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, touch.screenPosition,
                        InAppContext.SceneLoader.Camera, out var positionInRect) ? positionInRect : Vector2.zero)
                    .Average();
                var absDiff = touches.Select(touch => 
                    ((RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, touch.screenPosition,
                        InAppContext.SceneLoader.Camera, out var currentInRect) ? currentInRect : Vector2.zero) - center).Abs() 
                    - ((RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, touch.startScreenPosition,
                        InAppContext.SceneLoader.Camera, out var startInRect) ? startInRect : Vector2.zero) - center).Abs())
                    .Sum();
                
                _currentScale = (content.sizeDelta + absDiff).y / content.sizeDelta.y;
                
                content.sizeDelta = 
                    content.sizeDelta = new Vector2(content.sizeDelta.x, _standardHeight * _currentScale);
            }
            else
            {
                _currentScale = 1f;
            }
            
            AdjustContent();
        }

        private void AdjustContent()
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, 
                Mathf.Clamp(content.sizeDelta.y, viewport.rect.size.y, viewport.rect.size.y * scaleMax));
        }
    }
}