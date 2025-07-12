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
        [SerializeField, Range(0f, 1f)] private float mAlpha = 1f;

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

        // ReSharper disable once InconsistentNaming
        public float alpha
        {
            get => mAlpha;
            set
            {
                base.color = base.color.SetAlpha(value);
                mAlpha = value;
            }
        }
        
        public override Color color
        {
            get => base.color;
            set
            {
                if (!Mathf.Approximately(color.r, value.r) ||
                    !Mathf.Approximately(color.g, value.g) ||
                    !Mathf.Approximately(color.b, value.b))
                {
                    colorType = ColorOf.Custom;
                }

                mAlpha = value.a;
                base.color = value;
            }
        }

#if UNITY_EDITOR
        
        [CustomEditor(typeof(ImageRx))]
        [CanEditMultipleObjects]
        public class ImageRxEditor : ImageEditor
        {
            private SerializedProperty _colorProp;
            private SerializedProperty _alphaProp;
            protected override void OnEnable()
            {
                _colorProp = serializedObject.FindProperty("mColorType");
                _alphaProp = serializedObject.FindProperty("mAlpha");
                base.OnEnable();
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                EditorGUILayout.LabelField("基本的には以下から色を選択してください。", EditorStyles.label);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_colorProp);
                EditorGUILayout.PropertyField(_alphaProp);
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
            if (colorType is ColorOf.Custom)
            {
                base.color = color.SetAlpha(mAlpha);
            }
            else
            {
                base.color = AssetInEditor.Theme.GetColor(colorType).SetAlpha(mAlpha);
            }
            
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
            if (colorType is ColorOf.Custom)
            {
                base.color = base.color.SetAlpha(mAlpha);
            }
            else
            {
                base.color = InAppContext.Theme.GetColor(colorType).SetAlpha(mAlpha);
            }
        }
    }
}