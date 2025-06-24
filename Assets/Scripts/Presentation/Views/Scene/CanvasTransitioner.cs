using System;
using UnityEngine;

namespace Presentation.Views.Scene
{
    public class CanvasTransitioner : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;

        public void ShowCanvasFast()
        {
            canvasGroup.alpha = 1f;
            rootRect.anchoredPosition = Vector2.zero;
        }
        
        public void HideCanvasFast()
        {
            canvasGroup.alpha = 0f;
            rootRect.anchoredPosition = Vector2.one * 25565; // Out of screen
        }

        public void ShowCanvasWithAnimation(SceneTransition transition, Action completeCallback = null)
        {
            
        }

        public void HideCanvasWithAnimation(SceneTransition transition, Action completeCallback = null)
        {
            
        }
    }
}