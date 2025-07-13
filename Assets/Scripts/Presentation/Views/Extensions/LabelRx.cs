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
                if (!base.color.IsApproximatedTo(value))
                {
                    colorType = ColorOf.Custom;
                }

                base.color = value;
            }
        }

#if UNITY_EDITOR
        
        [CustomEditor(typeof(LabelRx))]
        [CanEditMultipleObjects]
        public class LabelRxEditor : TMP_EditorPanelUI
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
            base.color = colorType switch
            {
                ColorOf.Custom => color.SetAlpha(alpha),
                ColorOf.Transparent => Color.clear,
                _ => AssetInEditor.Theme.GetColor(colorType).SetAlpha(alpha)
            };

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
            base.color = colorType switch
            {
                ColorOf.Custom => color.SetAlpha(alpha),
                ColorOf.Transparent => Color.clear,
                _ => InAppContext.Theme.GetColor(colorType).SetAlpha(alpha)
            };
        }
    }
}