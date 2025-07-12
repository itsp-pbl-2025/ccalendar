using UnityEngine;

namespace Presentation.Views.Scene.Sample
{
    public class FadablePage : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransform;
        
        public RectTransform Rctf => rectTransform;

        public void SetAlpha(float value)
        {
            canvasGroup.alpha = value;
        }
    }
}