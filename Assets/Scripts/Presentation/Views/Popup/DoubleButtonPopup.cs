using System;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public class DoubleButtonPopup : PopupWindow
    {
        private static readonly Vector2 DefaultSize = new(800f, 320f);
        
        [SerializeField] private RectTransform windowRect;
        [SerializeField] private LabelRx descriptionText, positiveText, negativeText;
        private Action _onPositiveCallback, _onNegativeCallback;

        public void Init(string description,
            string positive, Action positiveCallback,
            string negative, Action negativeCallback)
        {
            descriptionText.text = description;
            positiveText.text = positive;
            negativeText.text = negative;
            _onPositiveCallback = positiveCallback;
            _onNegativeCallback = negativeCallback;
            
            descriptionText.ForceMeshUpdate();
            windowRect.sizeDelta = DefaultSize + Vector2.up * descriptionText.GetRenderedValues(false).y;
        }
        
        public override bool EnableClosingByButton()
        {
            return false;
        }

        public void CloseWindow(bool isPositive)
        {
            base.CloseWindow();
            
            var callback = isPositive ? _onPositiveCallback : _onNegativeCallback;
            callback?.Invoke();
        }
    }
}