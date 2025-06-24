using Presentation.Presenter;
using Presentation.Utilities;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace Presentation.Views.Extensions
{
    [ExecuteAlways]
    public class ImageRx : Image
    {
        [SerializeField] private ColorOf mColorType;

        // ReSharper disable once InconsistentNaming
        public ColorOf colorType
        {
            get => mColorType;
            set
            {
                mColorType = value;
                
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    RenewColorInEditor();
                    return;
                }
#endif
                RenewColor();
            }
        }

#if UNITY_EDITOR
        
        public override Color color
        {
            get => base.color;
            set
            {
                colorType = ColorOf.Custom;
                base.color = value; 
            }
        }
        
        [CustomEditor(typeof(ImageRx))]
        [CanEditMultipleObjects]
        public class ImageRxEditor : ImageEditor
        {
            private SerializedProperty _colorProp;
            protected override void OnEnable()
            {
                _colorProp = serializedObject.FindProperty("mColorType");
                base.OnEnable();
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                EditorGUILayout.LabelField("基本的には以下から色を選択してください。", EditorStyles.label);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_colorProp);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    foreach (var obj in targets)
                    {
                        if (obj is ImageRx rx)
                        {
                            rx.RenewColorInEditor();
                        }
                    }
                }
                EditorGUILayout.Separator();
                
                base.OnInspectorGUI();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            RenewColorInEditor();
        }

        private void RenewColorInEditor()
        {
            if (colorType is ColorOf.Custom) return;
            base.color = AssetInEditor.Theme.GetColor(colorType);
            
            SetVerticesDirty();
        }
        
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                RenewColorInEditor();
                return;
            }
#endif
            
            RenewColor();
        }

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                RenewColor();
                InAppContext.EventDispatcher.AddGlobalEventListener(this, GlobalEvent.OnThemeUpdated, _ =>
                {
                    RenewColor();
                });
            }
        }

        private void RenewColor()
        {
            if (colorType is ColorOf.Custom) return;
            base.color = InAppContext.Theme.GetColor(colorType);
        }
    }
}