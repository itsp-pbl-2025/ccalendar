using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Extensions
{
    public class ToggleWithLabel : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private ToggleRP toggle;
        [SerializeField] private LabelRx label;
        
        public ToggleRP Toggle => toggle;
        public RectTransform RectTransform => rectTransform;
        public LabelRx Label => label;
    }
}