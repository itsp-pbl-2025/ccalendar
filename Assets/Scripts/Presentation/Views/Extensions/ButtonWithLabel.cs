using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Extensions
{
    public class ButtonWithLabel : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private ButtonRP button;
        [SerializeField] private LabelRx label;
        
        public ButtonRP Button => button;
        public RectTransform RectTransform => rectTransform;
        public LabelRx Label => label;
    }
}