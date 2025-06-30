using System;
using System.Collections.Generic;
using Presentation.Utilities;
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
        [SerializeField] protected RectTransform centerContent;
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(ScalableScrollRect))]
        [CanEditMultipleObjects]
        public class ScalableScrollRectEditor : ScrollRectEditor
        {
            private SerializedProperty _centerContentProp;
            protected override void OnEnable()
            {
                _centerContentProp = serializedObject.FindProperty("centerContent");
                base.OnEnable();
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                EditorGUILayout.LabelField("カーソルの中心を指すオブジェクト。", EditorStyles.label);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_centerContentProp);
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
        
        private readonly HashSet<int> _pointerTouches = new();
        
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            _pointerTouches.Add(eventData.pointerId);
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            _pointerTouches.Remove(eventData.pointerId);
            base.OnDrag(eventData);
        }

        private void Update()
        {
            if (_pointerTouches.Count > 0)
            {
                var touches = Touch.activeTouches.AsValueEnumerable().Where(t => _pointerTouches.Contains(t.touchId));
                var touchStr = Touch.activeTouches.Count == 0 ? "null" : Touch.activeTouches.AsValueEnumerable().Select(x => x.touchId.ToString()).Aggregate((x, y) => $"{x}, {y}");
                var pointStr = _pointerTouches.Count == 0 ? "null" : _pointerTouches.AsValueEnumerable().Select(x => x.ToString()).Aggregate((x, y) => $"{x}, {y}");
                Debug.Log($"activeTouchIds: {touchStr}, pointerTouches: {pointStr}");
                if (touches.Count() > 0)
                {
                    var center = touches.Select(touch => touch.screenPosition).Average();
                    centerContent.anchoredPosition = center;
                }
            }
        }
    }
}