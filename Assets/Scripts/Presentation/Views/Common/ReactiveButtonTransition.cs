using System;
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
    public class ReactiveButtonTransition : MonoBehaviour
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
        
        [SerializeReference] private MonoBehaviour buttonRP;
        
        private IButtonState _targetButton;
        private IButtonState TargetButton => _targetButton ??= buttonRP as IButtonState;

        [SerializeField] private Transition transitionMode = Transition.ColorTint;

        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private ColorBlock colorTints = ColorBlock.defaultColorBlock;
        [SerializeField] private ColorTypeBlock colorTypes = ColorTypeBlock.defaultColorTypeBlock;
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
            private SerializedProperty _buttonProp;
            private SerializedProperty _modeProp;

            private void OnEnable()
            {
                _scriptProp = serializedObject.FindProperty("m_Script");
                _buttonProp = serializedObject.FindProperty("buttonRP");
                _modeProp = serializedObject.FindProperty("transitionMode");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(_scriptProp);
                EditorGUI.EndDisabledGroup();
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_buttonProp);
                if (EditorGUI.EndChangeCheck())
                {
                    UnityEngine.Object assignedObject = _buttonProp.objectReferenceValue;

                    if (assignedObject != null)
                    {
                        if (assignedObject is GameObject assignedGameObject)
                        {
                            var foundButtonState = assignedGameObject.GetComponent<IButtonState>() as MonoBehaviour;

                            if (foundButtonState == null)
                            {
                                Debug.LogError($"GameObject '{assignedGameObject.name}' does not contain any MonoBehaviour implementing IButtonState. Assigning null to {_buttonProp.name}.");
                                _buttonProp.objectReferenceValue = null;
                            }
                            else
                            {
                                Debug.LogWarning($"GameObject '{assignedGameObject.name}' was assigned. Automatically setting {_buttonProp.name} to the first found IButtonState implementer: '{foundButtonState.GetType().Name}'.");
                                _buttonProp.objectReferenceValue = foundButtonState;
                            }
                        }
                        else if (assignedObject is MonoBehaviour assignedMonoBehaviour)
                        {
                            // Case 2: ドラッグされたのが MonoBehaviour コンポーネントそのものの場合
                            if (assignedMonoBehaviour is not IButtonState)
                            {
                                if (assignedMonoBehaviour.gameObject.TryGetComponent(out IButtonState buttonState))
                                {
                                    var buttonStateObject = buttonState as MonoBehaviour;
                                    if (buttonStateObject)
                                    {
                                        _buttonProp.objectReferenceValue = buttonStateObject;
                                    }
                                    else
                                    {
                                        Debug.LogError($"'{assignedMonoBehaviour.name}' (Type: {assignedMonoBehaviour.GetType().Name}) does not implement IButtonState. Assigning null to {_buttonProp.name}.");
                                        _buttonProp.objectReferenceValue = null;
                                    }
                                }
                                else
                                {
                                    // IButtonState を実装していない MonoBehaviour がアサインされた場合
                                    Debug.LogError($"'{assignedMonoBehaviour.name}' (Type: {assignedMonoBehaviour.GetType().Name}) does not implement IButtonState. Assigning null to {_buttonProp.name}.");
                                    _buttonProp.objectReferenceValue = null;
                                }
                            }
                            else
                            {
                                // 適切に IButtonState を実装しているので、そのまま受け入れる
                                // この場合、_buttonProp.objectReferenceValue は既に assignedMonoBehaviour を指している
                                // ので、明示的な再代入は不要だが、処理の流れとして代入しても問題はない
                                // foundButtonStateMonoBehaviour = assignedMonoBehaviour; // 必要であれば値を保持
                            }
                        }
                        else
                        {
                            // MonoBehaviour でも GameObject でもないオブジェクトがアサインされた場合
                            Debug.LogError($"Assigned object '{assignedObject.name}' is not a GameObject or MonoBehaviour. Assigning null to {_buttonProp.name}.");
                            _buttonProp.objectReferenceValue = null;
                        }
                    }

                    serializedObject.ApplyModifiedProperties();
                }
                
                EditorGUILayout.PropertyField(_modeProp);

                switch ((Transition)_modeProp.enumValueIndex)
                {
                    case Transition.ColorTint:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "buttonRP", "transitionMode", "colorTypes", "colorAssigns", "targetImage", "spriteState", "animator", "animationTriggers");
                        break;
                    case Transition.ColorType:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "buttonRP", "transitionMode", "colorTints", "colorAssigns", "targetImage", "spriteState", "animator", "animationTriggers");
                        break;
                    case Transition.ColorAssign:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "buttonRP", "transitionMode", "colorTints", "colorTypes", "targetImage", "spriteState", "animator", "animationTriggers");
                        break;
                    case Transition.SpriteSwap:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "buttonRP", "transitionMode", "targetGraphic", "colorTints", "colorTypes", "colorAssigns", "animator", "animationTriggers");
                        break;
                    case Transition.Animation:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "buttonRP", "transitionMode", "targetGraphic", "colorTints", "colorTypes", "colorAssigns", "targetImage", "spriteState");
                        break;
                    default:
                        DrawPropertiesExcluding(serializedObject,
                            "m_Script", "buttonRP", "transitionMode", "targetGraphic", "colorTints", "colorTypes", "colorAssigns", "targetImage", "spriteState", "animator", "animationTriggers");
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
            foreach (var child in transform.GetComponentsInChildren<MonoBehaviour>())
            {
                if (child is not IButtonState buton) continue;
                buttonRP = child;
                _targetButton = buton;
            }
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
                    TargetButton.State
                        .Subscribe(t => ApplyColorTint(t.state, t.instant))
                        .AddTo(this);
                    break;
                case Transition.ColorType:
                    if (!targetGraphic)
                    {
                        Debug.LogError("Color Type Transition is enabled, but no Graphic is assigned.");
                        return;
                    }
                    TargetButton.State
                        .Subscribe(t => ApplyColorType(t.state, t.instant))
                        .AddTo(this);
                    break;
                case Transition.ColorAssign:
                    if (!targetGraphic)
                    {
                        Debug.LogError("Color Assign Transition is enabled, but no Graphic is assigned.");
                        return;
                    }
                    TargetButton.State
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
                    TargetButton.State
                        .Subscribe(t => ApplySpriteSwap(t.state))
                        .AddTo(this);
                    break;
                case Transition.Animation:
                    if (!animator)
                    {
                        Debug.LogError("Animation Transition is enabled, but no Animator is assigned.");
                        return;
                    }
                    TargetButton.State
                        .Subscribe(t => ApplyAnimation(t.state))
                        .AddTo(this);
                    break;
                default:
                    // nothing to do
                    break;
            }
        }

        private void ApplyColorTint(ButtonSelectionState state, bool instant)
        {
            var to = state switch
            {
                ButtonSelectionState.Normal => colorTints.normalColor,
                ButtonSelectionState.Highlighted => colorTints.highlightedColor,
                ButtonSelectionState.Pressed => colorTints.pressedColor,
                ButtonSelectionState.Selected => colorTints.selectedColor,
                _ => colorTints.disabledColor,
            } * colorTints.colorMultiplier;

            targetGraphic.CrossFadeColor(
                to,
                instant ? 0f : colorTints.fadeDuration,
                true, true
            );
        }

        private Sequence _seq;
        private void ApplyColorType(ButtonSelectionState state, bool instant)
        {
            var to = state switch
            {
                ButtonSelectionState.Normal => colorTypes.normalColor,
                ButtonSelectionState.Highlighted => colorTypes.highlightedColor,
                ButtonSelectionState.Pressed => colorTypes.pressedColor,
                ButtonSelectionState.Selected => colorTypes.selectedColor,
                _ => colorTypes.disabledColor,
            };

            var fromColor = targetGraphic.color;
            var toColor = to switch
            {
                ColorOf.Custom => fromColor.SetAlpha(targetGraphic.color.a),
                ColorOf.Transparent => Color.clear, 
                _ => InAppContext.Theme.GetColor(to).SetAlpha(targetGraphic.color.a)
            };

            if (instant)
            {
                AtCompletedState();
            }
            else
            {
                _seq = DOTween.Sequence()
                    .Append(DOVirtual.Color(fromColor, toColor, colorTypes.fadeDuration, c => targetGraphic.color = c))
                    .OnComplete(AtCompletedState);
            }

            return;

            void AtCompletedState()
            {
                if (targetGraphic is ImageRx imageRx)
                    imageRx.colorType = to;
                else if (targetGraphic is LabelRx labelRx)
                    labelRx.colorType = to;
                else
                    targetGraphic.color = toColor;
            }
        }
        
        private void ApplyColorAssign(ButtonSelectionState state, bool instant)
        {
            _seq?.Kill();
            
            var to = state switch
            {
                ButtonSelectionState.Normal => colorAssigns.normalColor,
                ButtonSelectionState.Highlighted => colorAssigns.highlightedColor,
                ButtonSelectionState.Pressed => colorAssigns.pressedColor,
                ButtonSelectionState.Selected => colorAssigns.selectedColor,
                _ => colorAssigns.disabledColor,
            };
            
            _seq = DOTween.Sequence().Append(DOVirtual.Color(targetGraphic.color, to, instant ? 0f : colorAssigns.fadeDuration, c => targetGraphic.color = c));
        }

        private void ApplySpriteSwap(ButtonSelectionState state)
        {
            targetImage.sprite = state switch
            {
                ButtonSelectionState.Normal => _originalSprite,
                ButtonSelectionState.Highlighted => spriteState.highlightedSprite
                    ? spriteState.highlightedSprite : _originalSprite,
                ButtonSelectionState.Pressed => spriteState.pressedSprite
                    ? spriteState.pressedSprite : _originalSprite,
                ButtonSelectionState.Selected => spriteState.highlightedSprite
                    ? spriteState.highlightedSprite : _originalSprite,
                _ => spriteState.disabledSprite
                    ? spriteState.disabledSprite : _originalSprite,
            };
        }
        
        private void ApplyAnimation(ButtonSelectionState state)
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
                ButtonSelectionState.Normal => animationTriggers.normalTrigger,
                ButtonSelectionState.Highlighted => animationTriggers.highlightedTrigger,
                ButtonSelectionState.Pressed => animationTriggers.pressedTrigger,
                ButtonSelectionState.Selected => animationTriggers.selectedTrigger,
                _ => animationTriggers.disabledTrigger,
            };
            
            animator.SetTrigger(animationTrigger);
            _prevAnimationTrigger = animationTrigger;
        }
    }
}