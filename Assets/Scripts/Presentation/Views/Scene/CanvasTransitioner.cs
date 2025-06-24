using System;
using DG.Tweening;
using UnityEngine;

namespace Presentation.Views.Scene
{
    public class CanvasTransitioner : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;

        private Sequence _seq;
        private SceneTransition _transition;

        private void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
        }
        
        public void ShowCanvasFast()
        {
            _seq?.Complete();
            _transition = null;
            
            canvasGroup.alpha = 1f;
            rootRect.anchoredPosition = Vector2.zero;
        }
        
        public void HideCanvasFast()
        {
            _seq?.Complete();
            _transition = null;

            canvasGroup.alpha = 0f;
            rootRect.anchoredPosition = Vector2.one * 25565; // Out of screen
        }

        public void ShowCanvasWithAnimation(SceneTransition transition, Action completeCallback = null)
        {
            _seq?.Complete();
            _transition = transition;
            
            _seq = DOTween.Sequence().AppendInterval(transition.TimeBeforeIn);

            switch (transition.Transition)
            {
                case TransitionType.None:
                    _seq.AppendCallback(() =>
                        {
                            canvasGroup.alpha = 1f;
                            rootRect.anchoredPosition = Vector2.zero;
                        }).AppendInterval(transition.TimeTransitionIn);
                    break;
                case TransitionType.Fade:
                    _seq.AppendCallback(() =>
                        {
                            rootRect.anchoredPosition = -1 * transition.DirectionIn * rootRect.sizeDelta;
                        })
                        .Append(rootRect.DOAnchorPos(Vector2.zero, transition.TimeTransitionIn).SetEase(transition.EasingIn))
                        .Join(DOVirtual.Float(0f, 1f, transition.TimeTransitionIn, SetAlpha).SetEase(transition.EasingIn));
                    break;
                case TransitionType.Slide:
                    _seq.AppendCallback(() =>
                        {
                            rootRect.anchoredPosition = -1 * transition.DirectionIn * rootRect.sizeDelta;
                        })
                        .Append(rootRect.DOAnchorPos(Vector2.zero, transition.TimeTransitionIn).SetEase(transition.EasingIn));
                    break;
                case TransitionType.BlackOut:
                    break;
            }
            
            _seq.OnComplete(() => completeCallback?.Invoke());
        }

        public void HideCanvasWithAnimation(SceneTransition transition, Action completeCallback = null)
        {
            _seq?.Complete();
            _transition = transition;
            
            _seq = DOTween.Sequence().AppendInterval(transition.TimeBeforeOut);

            switch (transition.Transition)
            {
                case TransitionType.None:
                    _seq.AppendCallback(() =>
                    {
                        canvasGroup.alpha = 0f;
                        rootRect.anchoredPosition =Vector2.one * 25565; // Out of screen
                    }).AppendInterval(transition.TimeTransitionOut);
                    break;
                case TransitionType.Fade:
                    _seq.AppendCallback(() =>
                        {
                            rootRect.anchoredPosition = Vector2.zero;
                        })
                        .Append(rootRect.DOAnchorPos(transition.DirectionIn * rootRect.sizeDelta, transition.TimeTransitionOut).SetEase(transition.EasingOut))
                        .Join(DOVirtual.Float(1f, 0f, transition.TimeTransitionOut, SetAlpha).SetEase(transition.EasingOut));
                    break;
                case TransitionType.Slide:
                    _seq.AppendCallback(() =>
                        {
                            rootRect.anchoredPosition = Vector2.zero;
                        })
                        .Append(rootRect.DOAnchorPos(transition.DirectionIn * rootRect.sizeDelta, transition.TimeTransitionOut).SetEase(transition.EasingOut));
                    break;
                case TransitionType.BlackOut:
                    break;
            }
            
            _seq.OnComplete(() => completeCallback?.Invoke());
        }
    }
}