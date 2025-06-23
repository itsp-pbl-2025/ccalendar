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
        
        [CustomEditor(typeof(ImageRx))]
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

                    var imageRx = (ImageRx)target;
                    if (imageRx)
                    {
                        imageRx.RenewColorInEditor();
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
            color = AssetInEditor.Theme.GetColor(colorType);
            
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
            color = InAppContext.Theme.GetColor(colorType);
        }
    }
}