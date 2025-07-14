using DG.Tweening;
using Presentation.Presenter;
using Presentation.Utilities;
using Presentation.Views.Extensions;
using R3;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Presentation.Views.Common
{
    public class ReactiveToggleTransition : MonoBehaviour
    {
        private enum Transition
        {
            None,
            ColorTint,
            ColorType,
            ColorAssign,
            SpriteSwap,
            Animation
        }
        
        
        [SerializeReference] private ToggleRP toggleRP;

        [SerializeField] private Transition transitionMode = Transition.ColorTint;

        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private Color tintColorOn = Color.white, tintColorOff = Color.white;
        [SerializeField] private float tintTransitionDuration = 0.1f;
        [SerializeField] private ColorOf typeColorOn = ColorOf.Primary;
        [SerializeField, Range(0f, 1f)] private float typeAlphaOn = 1f;
        [SerializeField] private ColorOf typeColorOff = ColorOf.Transparent;
        [SerializeField, Range(0f, 1f)] private float typeAlphaOff = 1f;
        [SerializeField] private float typeTransitionDuration = 0.1f;
        [SerializeField] private Color assignColorOn = Color.white, assignColorOff = Color.white;
        [SerializeField] private float assignTransitionDuration = 0.1f;

        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite spriteOn, spriteOff;

        [SerializeField] private Animator animator;
        [SerializeField] private string animOn, animOff;

        private Sprite _originalSprite;
        private string _prevAnimationTrigger = "";
        
        #region Editor Extensions
        
#if UNITY_EDITOR

        [CustomEditor(typeof(ReactiveToggleTransition))]
        public class ReactiveButtonTransitionEditor : Editor
        {
            private SerializedProperty _scriptProp;
            private SerializedProperty _toggleProp;
            private SerializedProperty _modeProp;

            private void OnEnable()
            {
                _scriptProp = serializedObject.FindProperty("m_Script");
                _toggleProp = serializedObject.FindProperty("toggleRP");
                _modeProp = serializedObject.FindProperty("transitionMode");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(_scriptProp);
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.PropertyField(_toggleProp);
                EditorGUILayout.PropertyField(_modeProp);
                switch ((Transition)_modeProp.enumValueIndex)
                {
                    case Transition.ColorTint:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "toggleRP", "transitionMode", "targetImage", "animator", "typeColorOn", "typeAlphaOn", "typeColorOff", "typeAlphaOff", "typeTransitionDuration", "assignColorOn", "assignColorOff", "assignTransitionDuration", "spriteOn", "spriteOff", "animOn", "animOff");
                        break;
                    case Transition.ColorType:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "toggleRP", "transitionMode", "targetImage", "animator", "tintColorOn", "tintColorOff", "tintTransitionDuration", "assignColorOn", "assignColorOff", "assignTransitionDuration", "spriteOn", "spriteOff", "animOn", "animOff");
                        break;
                    case Transition.ColorAssign:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "toggleRP", "transitionMode", "targetImage", "animator", "tintColorOn", "tintColorOff", "tintTransitionDuration", "typeColorOn", "typeAlphaOn", "typeColorOff", "typeAlphaOff", "typeTransitionDuration", "spriteOn", "spriteOff", "animOn", "animOff");
                        break;
                    case Transition.SpriteSwap:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "toggleRP", "transitionMode", "targetGraphic", "animator", "tintColorOn", "tintColorOff", "tintTransitionDuration", "typeColorOn", "typeAlphaOn", "typeColorOff", "typeAlphaOff", "typeTransitionDuration", "assignColorOn", "assignColorOff", "assignTransitionDuration", "animOn", "animOff");
                        break;
                    case Transition.Animation:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "toggleRP", "transitionMode", "targetGraphic", "targetImage", "tintColorOn", "tintColorOff", "tintTransitionDuration", "typeColorOn", "typeAlphaOn", "typeColorOff", "typeAlphaOff", "typeTransitionDuration", "assignColorOn", "assignColorOff", "assignTransitionDuration", "spriteOn", "spriteOff");
                        break;
                    default:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "toggleRP", "transitionMode", "targetGraphic", "targetImage", "animator", "tintColorOn", "tintTransitionDuration", "tintColorOff", "typeColorOn", "typeTransitionDuration", "typeColorOff", "assignColorOn", "assignTransitionDuration", "assignColorOff", "spriteOn", "spriteOff", "animOn", "animOff");
                        break;
                }
                
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        /// <summary>
        /// Reset() is called after this component is attached.
        /// </summary>
        private void Reset()
        {
            toggleRP = GetComponentInParent<ToggleRP>();
            targetGraphic = TryGetComponent<Graphic>(out var g) ? g : null;
            targetImage = TryGetComponent<Image>(out var i) ? i : null;
            animator = TryGetComponent<Animator>(out var a) ? a : null;
            if (toggleRP && targetGraphic)
            {
                if (toggleRP.isOn)
                {
                    assignColorOn = targetGraphic.color;
                    typeAlphaOn = targetGraphic.color.a;
                }
                else
                {
                    assignColorOff = targetGraphic.color;
                    typeAlphaOff = targetGraphic.color.a;
                }
            }
            EditorUtility.SetDirty(this);
        }
#endif

        #endregion

        private void Awake()
        {
            switch (transitionMode)
            {
                case Transition.ColorTint:
                    if (!targetGraphic)
                    {
                        Debug.LogError("Color Tint Transition is enabled, but no Graphic is assigned.");
                        return;
                    }
                    ApplyColorTint(toggleRP.isOn, true);
                    toggleRP.Toggled
                        .Subscribe(t => ApplyColorTint(t.toggled, t.instant))
                        .AddTo(this);
                    break;
                case Transition.ColorType:
                    if (!targetGraphic)
                    {
                        Debug.LogError("Color Type Transition is enabled, but no Graphic is assigned.");
                        return;
                    }
                    ApplyColorType(toggleRP.isOn, true);
                    toggleRP.Toggled
                        .Subscribe(t => ApplyColorType(t.toggled, t.instant))
                        .AddTo(this);
                    break;
                case Transition.ColorAssign:
                    if (!targetGraphic)
                    {
                        Debug.LogError("Color Assign Transition is enabled, but no Graphic is assigned.");
                        return;
                    }
                    ApplyColorAssign(toggleRP.isOn, true);
                    toggleRP.Toggled
                        .Subscribe(t => ApplyColorAssign(t.toggled, t.instant))
                        .AddTo(this);
                    break;
                case Transition.SpriteSwap:
                    if (!targetImage)
                    {
                        Debug.LogError("Sprite Swap Transition is enabled, but no Image is assigned.");
                        return;   
                    }
                    _originalSprite = targetImage.sprite;
                    ApplySpriteSwap(toggleRP.isOn);
                    toggleRP.Toggled
                        .Subscribe(t => ApplySpriteSwap(t.toggled))
                        .AddTo(this);
                    break;
                case Transition.Animation:
                    if (!animator)
                    {
                        Debug.LogError("Animation Transition is enabled, but no Animator is assigned.");
                        return;
                    }
                    ApplyAnimation(toggleRP.isOn);
                    toggleRP.Toggled
                        .Subscribe(t => ApplyAnimation(t.toggled))
                        .AddTo(this);
                    break;
                default:
                    // nothing to do
                    break;
            }
        }

        private void ApplyColorTint(bool toggled, bool instant)
        {
            targetGraphic.CrossFadeColor(
                toggled ? tintColorOn : tintColorOff,
                instant ? 0f : tintTransitionDuration,
                true, true
            );
        }

        private Sequence _seq;
        private void ApplyColorType(bool toggled, bool instant)
        {
            var to = toggled ? typeColorOn : typeColorOff;
            var toAlpha = toggled ? typeAlphaOn : typeAlphaOff;

            var fromColor = targetGraphic.color;
            var toColor = to switch
            {
                ColorOf.Custom => fromColor.SetAlpha(toAlpha),
                ColorOf.Transparent => Color.clear, 
                _ => InAppContext.Theme.GetColor(to).SetAlpha(toAlpha)
            };

            if (instant)
            {
                AtCompletedState();
            }
            else
            {
                _seq = DOTween.Sequence()
                    .Append(DOVirtual.Color(fromColor, toColor, typeTransitionDuration, c => targetGraphic.color = c))
                    .OnComplete(AtCompletedState);
            }

            return;

            void AtCompletedState()
            {
                if (targetGraphic is ImageRx imageRx)
                {
                    if (to is not ColorOf.Transparent) imageRx.alpha = toAlpha;
                    imageRx.colorType = to;
                }
                else if (targetGraphic is LabelRx labelRx)
                {
                    if (to is not ColorOf.Transparent) labelRx.alpha = toAlpha;
                    labelRx.colorType = to;
                }
                else
                {
                    targetGraphic.color = toColor;
                }
            }
        }
        
        private void ApplyColorAssign(bool toggled, bool instant)
        {
            _seq?.Kill();
            
            _seq = DOTween.Sequence().Append(DOVirtual.Color(targetGraphic.color, toggled ? assignColorOn : assignColorOff, instant ? 0f : assignTransitionDuration, c => targetGraphic.color = c));
        }

        private void ApplySpriteSwap(bool toggled)
        {
            targetImage.sprite = toggled ? spriteOn ? spriteOn : _originalSprite : spriteOff ? spriteOff : _originalSprite;
        }
        
        private void ApplyAnimation(bool toggled)
        {
            if (_prevAnimationTrigger != "")
            {
                animator.ResetTrigger(_prevAnimationTrigger);
            }
            animator.ResetTrigger(animOn);
            animator.ResetTrigger(animOff);

            var animationTrigger = toggled ? animOn : animOff;
            
            animator.SetTrigger(animationTrigger);
            _prevAnimationTrigger = animationTrigger;
        }
    }
}