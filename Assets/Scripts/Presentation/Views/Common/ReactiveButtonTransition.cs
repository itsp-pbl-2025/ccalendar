using System;
using DG.Tweening;
using Presentation.Views.Extensions;
using R3;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Presentation.Views.Common
{
    public class ReactiveButtonTransition : MonoBehaviour
    {
        private enum Transition
        {
            None,
            ColorTint,
            ColorAssign,
            SpriteSwap,
            Animation
        }
        
        [SerializeField] private ButtonRP buttonRP;

        [SerializeField] private Transition transitionMode = Transition.ColorTint;

        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private ColorBlock colorTints = ColorBlock.defaultColorBlock;
        [SerializeField] private ColorBlock colorAssigns = ColorBlock.defaultColorBlock;

        [SerializeField] private Image targetImage;
        [SerializeField] private SpriteState spriteState;

        [SerializeField] private Animator animator;
        [SerializeField] private AnimationTriggers animationTriggers = new AnimationTriggers();

        private Sprite _originalSprite;
        private string _prevAnimationTrigger = "";

        #region Editor Extensions
        
#if UNITY_EDITOR

        [CustomEditor(typeof(ReactiveButtonTransition))]
        public class ReactiveButtonTransitionEditor : Editor
        {
            private SerializedProperty _scriptProp;
            private SerializedProperty _modeProp;

            private void OnEnable()
            {
                _scriptProp = serializedObject.FindProperty("m_Script");
                _modeProp = serializedObject.FindProperty("transitionMode");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(_scriptProp);
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.PropertyField(_modeProp);

                switch ((Transition)_modeProp.enumValueIndex)
                {
                    case Transition.ColorTint:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "transitionMode", "colorAssigns", "targetImage", "spriteState", "animator", "animationTriggers");
                        break;
                    case Transition.ColorAssign:
                        
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "transitionMode", "colorTints", "targetImage", "spriteState", "animator", "animationTriggers");
                        break;
                    case Transition.SpriteSwap:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "transitionMode", "targetGraphic", "colorTints", "colorAssigns", "animator", "animationTriggers");
                        break;
                    case Transition.Animation:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "transitionMode", "targetGraphic", "colorTints", "colorAssigns", "targetImage", "spriteState");
                        break;
                    default:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "transitionMode", "targetGraphic", "colorTints", "colorAssigns", "targetImage", "spriteState", "animator", "animationTriggers");
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
            buttonRP = GetComponentInParent<ButtonRP>();
            targetGraphic = TryGetComponent<Graphic>(out var g) ? g : null;
            targetImage = TryGetComponent<Image>(out var i) ? i : null;
            animator = TryGetComponent<Animator>(out var a) ? a : null;
            if (targetGraphic) colorAssigns.normalColor = targetGraphic.color;
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
                    buttonRP.State
                        .Subscribe(t => ApplyColorTint(t.state, t.instant))
                        .AddTo(this);
                    break;
                case Transition.ColorAssign:
                    if (!targetGraphic)
                    {
                        Debug.LogError("Color Assign Transition is enabled, but no Graphic is assigned.");
                        return;
                    }
                    buttonRP.State
                        .Subscribe(t => ApplyColorAssign(t.state, t.instant))
                        .AddTo(this);
                    break;
                case Transition.SpriteSwap:
                    if (!targetImage)
                    {
                        Debug.LogError("Sprite Swap Transition is enabled, but no Image is assigned.");
                        return;   
                    }
                    _originalSprite = targetImage.sprite;
                    buttonRP.State
                        .Subscribe(t => ApplySpriteSwap(t.state))
                        .AddTo(this);
                    break;
                case Transition.Animation:
                    if (!animator)
                    {
                        Debug.LogError("Animation Transition is enabled, but no Animator is assigned.");
                        return;
                    }
                    buttonRP.State
                        .Subscribe(t => ApplyAnimation(t.state))
                        .AddTo(this);
                    break;
                default:
                    // nothing to do
                    break;
            }
        }

        private void ApplyColorTint(ButtonRP.ButtonSelectionState state, bool instant)
        {
            var to = state switch
            {
                ButtonRP.ButtonSelectionState.Normal => colorTints.normalColor,
                ButtonRP.ButtonSelectionState.Highlighted => colorTints.highlightedColor,
                ButtonRP.ButtonSelectionState.Pressed => colorTints.pressedColor,
                ButtonRP.ButtonSelectionState.Selected => colorTints.selectedColor,
                _ => colorTints.disabledColor,
            } * colorTints.colorMultiplier;

            targetGraphic.CrossFadeColor(
                to,
                instant ? 0f : colorTints.fadeDuration,
                true, true
            );
        }

        private Sequence _seq;
        private void ApplyColorAssign(ButtonRP.ButtonSelectionState state, bool instant)
        {
            _seq?.Kill();
            
            var to = state switch
            {
                ButtonRP.ButtonSelectionState.Normal => colorAssigns.normalColor,
                ButtonRP.ButtonSelectionState.Highlighted => colorAssigns.highlightedColor,
                ButtonRP.ButtonSelectionState.Pressed => colorAssigns.pressedColor,
                ButtonRP.ButtonSelectionState.Selected => colorAssigns.selectedColor,
                _ => colorAssigns.disabledColor,
            };
            
            _seq = DOTween.Sequence().Append(DOVirtual.Color(targetGraphic.color, to, instant ? 0f : colorAssigns.fadeDuration, c => targetGraphic.color = c));
        }

        private void ApplySpriteSwap(ButtonRP.ButtonSelectionState state)
        {
            targetImage.sprite = state switch
            {
                ButtonRP.ButtonSelectionState.Normal => _originalSprite,
                ButtonRP.ButtonSelectionState.Highlighted => spriteState.highlightedSprite
                    ? spriteState.highlightedSprite : _originalSprite,
                ButtonRP.ButtonSelectionState.Pressed => spriteState.pressedSprite
                    ? spriteState.pressedSprite : _originalSprite,
                ButtonRP.ButtonSelectionState.Selected => spriteState.highlightedSprite
                    ? spriteState.highlightedSprite : _originalSprite,
                _ => spriteState.disabledSprite
                    ? spriteState.disabledSprite : _originalSprite,
            };
        }
        
        private void ApplyAnimation(ButtonRP.ButtonSelectionState state)
        {
            if (_prevAnimationTrigger != "")
            {
                animator.ResetTrigger(_prevAnimationTrigger);
            }
            animator.ResetTrigger(animationTriggers.normalTrigger);
            animator.ResetTrigger(animationTriggers.highlightedTrigger);
            animator.ResetTrigger(animationTriggers.pressedTrigger);
            animator.ResetTrigger(animationTriggers.selectedTrigger);
            animator.ResetTrigger(animationTriggers.disabledTrigger);
            
            var animationTrigger = state switch
            {
                ButtonRP.ButtonSelectionState.Normal => animationTriggers.normalTrigger,
                ButtonRP.ButtonSelectionState.Highlighted => animationTriggers.highlightedTrigger,
                ButtonRP.ButtonSelectionState.Pressed => animationTriggers.pressedTrigger,
                ButtonRP.ButtonSelectionState.Selected => animationTriggers.selectedTrigger,
                _ => animationTriggers.disabledTrigger,
            };
            
            animator.SetTrigger(animationTrigger);
            _prevAnimationTrigger = animationTrigger;
        }
    }
}