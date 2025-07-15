using DG.Tweening;
using UnityEngine;

namespace Presentation.Views.Scene
{
    public enum TransitionType
    {
        None,
        Fade,
        Slide,
        BlackOut,
    }
    
    public record SceneTransition
    {
        public const float TimeTransitionDefault = 0.333f;
        public TransitionType Transition;
        public float TimeBeforeIn = 0.0f, TimeTransitionIn = TimeTransitionDefault;
        public Vector2 DirectionIn;
        public Ease EasingIn = Ease.InOutQuad;
        public float TimeBeforeOut = 0.0f, TimeTransitionOut = TimeTransitionDefault;
        public Vector2 DirectionOut;
        public Ease EasingOut = Ease.InOutQuad;

        public SceneTransition(SceneTransition transition)
        {
            Transition = transition.Transition;
            TimeBeforeIn = transition.TimeBeforeIn;
            TimeTransitionIn = transition.TimeTransitionIn;
            DirectionIn = transition.DirectionIn;
            EasingIn = transition.EasingIn;
            TimeBeforeOut = transition.TimeBeforeOut;
            TimeTransitionOut = transition.TimeTransitionOut;
            DirectionOut = transition.DirectionOut;
            EasingOut = transition.EasingOut;
        }

        #region TransitionPreset

        public static SceneTransition FadeLeft = new()
        {
            Transition = TransitionType.Fade,
            DirectionIn = Vector2.left,
            DirectionOut = Vector2.left,
        };
        public static SceneTransition FadeRight = new()
        {
            Transition = TransitionType.Fade,
            DirectionIn = Vector2.right,
            DirectionOut = Vector2.right,
        };
        
        public static SceneTransition SlideLeft = new()
        {
            Transition = TransitionType.Slide,
            DirectionIn = Vector2.left,
            EasingIn = Ease.OutQuad,
            DirectionOut = Vector2.left,
            EasingOut = Ease.OutQuad,
        };
        public static SceneTransition SlideRight = new()
        {
            Transition = TransitionType.Slide,
            DirectionIn = Vector2.right,
            EasingIn = Ease.OutQuad,
            DirectionOut = Vector2.right,
            EasingOut = Ease.OutQuad,
        };

        #endregion
    }
}