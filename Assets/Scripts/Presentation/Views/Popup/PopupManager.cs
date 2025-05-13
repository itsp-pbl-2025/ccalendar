using System;
using System.Collections.Generic;
using Presentation.Views.Common;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public class PopupManager : MonoBehaviour
    {
        private static PopupManager _instance;
        public static PopupManager Instance => _instance ? _instance : _instance ??= FindAnyObjectByType<PopupManager>();

        [SerializeField] private Transform popupParent;
        [SerializeField] private AutoAspectCanvas autoAspectCanvas;
        [SerializeField] private SingleButtonPopup prefabSingleButtonPopup;
        [SerializeField] private DoubleButtonPopup prefabDoubleButtonPopup;

        private readonly Stack<PopupWindow> _showingPopups = new();
        
        public T ShowPopup<T>(T popup) where T : PopupWindow
        {
            var window = Instantiate(popup, popupParent);
            window.OnOpenWindow();
            window.SetupWithCanvas(autoAspectCanvas);
            _showingPopups.Push(window);
            return window;
        }

        public bool ShowPopupUnique<T>(T popup, out T window) where T : PopupWindow
        {
            window = null;
            foreach (var exist in _showingPopups)
            {
                if (exist is T) return false;
            }

            window = ShowPopup(popup);
            return true;
        }

        public bool CheckPopupExist(Type popupType)
        {
            foreach (var exist in _showingPopups)
            {
                if (exist.GetType() == popupType) return true;
            }

            return false;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                var window = _showingPopups.Pop();
                if (window.EnableClosingByButton())
                {
                    window.CloseWindow();
                    Destroy(window.gameObject);
                    Destroy(window);
                }
            }
        }

        public bool ClosePopup(PopupWindow window)
        {
            if (!_showingPopups.TryPeek(out var front)) return false;
            if (front != window) return false;
            
            _showingPopups.Pop();
            window.CloseWindow();
            window.UnsetFromCanvas(autoAspectCanvas);
            Destroy(window.gameObject);
            Destroy(window);
            return true;
        }

        public void ShowSinglePopup(string description, string ok = "OK", Action callback = null)
        {
            var window = ShowPopup(prefabSingleButtonPopup);
            window.Init(description, ok, callback);
        }

        public void ShowDoublePopup(string description,
            string positive = "Accept", Action positiveCallback = null,
            string negative = "Cancel", Action negativeCallback = null)
        {
            var window = ShowPopup(prefabDoubleButtonPopup);
            window.Init(description, positive, positiveCallback, negative, negativeCallback);
        }
    }
}