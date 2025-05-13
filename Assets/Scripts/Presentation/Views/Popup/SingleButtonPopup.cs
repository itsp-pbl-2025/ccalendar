using System;
using TMPro;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public class SingleButtonPopup : PopupWindow
    {
        private static readonly Vector2 DefaultSize = new(640f, 320f);
        
        [SerializeField] private RectTransform windowRect;
        [SerializeField] private TextMeshProUGUI descriptionText, okText;
        private Action _onClosingCallback;

        public void Init(string description, string ok, Action callback)
        {
            descriptionText.text = description;
            okText.text = ok;
            _onClosingCallback = callback;
            
            descriptionText.ForceMeshUpdate();
            windowRect.sizeDelta = DefaultSize + Vector2.up * descriptionText.GetRenderedValues(false).y;
        }
        
        public override bool EnableClosingByButton()
        {
            return false;
        }

        public override void CloseWindow()
        {
            base.CloseWindow();
            _onClosingCallback?.Invoke();
        }
    }
}