using Presentation.Presenter;
using Presentation.Utilities;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using TMPro.EditorUtilities;
#endif

namespace Presentation.Views.Extensions
{
    [ExecuteAlways]
    public class LabelRx : TextMeshProUGUI
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
        
        [CustomEditor(typeof(LabelRx))]
        [CanEditMultipleObjects]
        public class LabelRxEditor : TMP_EditorPanelUI
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
                        if (obj is LabelRx rx)
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
                color = color.SetAlpha(mAlpha);
            }
            else
            {
                color = AssetInEditor.Theme.GetColor(colorType).SetAlpha(mAlpha);
            }

            if (!font)
            {
                font = TMP_Settings.defaultFontAsset;
            }

            if (!fontSharedMaterial)
            {
                fontSharedMaterial = font.material;
            }
            
            SetVerticesDirty();
            ForceMeshUpdate();
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
                color = color.SetAlpha(mAlpha);
            }
            else
            {
                color = InAppContext.Theme.GetColor(colorType).SetAlpha(mAlpha);
            }
        }
    }
}