﻿using System;
using System.Collections.Generic;
using Presentation.Presenter;
using Presentation.Views.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation.Views.Popup
{
    public class PopupManager : MonoBehaviour
    {
        private static PopupManager _instance;
        public static PopupManager Instance => _instance ? _instance : _instance ??= FindAnyObjectByType<PopupManager>();

        [SerializeField] private Transform popupParent;
        [SerializeField] private AutoAspectCanvas autoAspectCanvas;
        
        private SingleButtonPopup _prefabSingleButtonPopup;
        private DoubleButtonPopup _prefabDoubleButtonPopup;
        
        private readonly List<PopupWindow> _showingPopups = new();
        
        public T ShowPopup<T>(T popup) where T : PopupWindow
        {
            var window = Instantiate(popup, popupParent);
            window.SetupWithCanvas(autoAspectCanvas);
            window.OnOpenWindow();
            _showingPopups.Add(window);
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
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            {
                var lastPopup = _showingPopups[^1];
                if (_showingPopups.Count > 0 && lastPopup.EnableClosingByButton())
                {
                    ClosePopup(lastPopup);
                }
            }
        }

        public bool ClosePopup(PopupWindow window)
        {
            if (_showingPopups.Count == 0) return false;
            if (!_showingPopups.Remove(window)) return false;
            
            window.CloseWindow();
            window.UnsetFromCanvas(autoAspectCanvas);
            Destroy(window.gameObject);
            Destroy(window);
            return true;
        }

        public void ShowSinglePopup(string description, string ok = "OK", Action callback = null)
        {
            _prefabSingleButtonPopup ??= InAppContext.Prefabs.GetPopup<SingleButtonPopup>();
            var window = ShowPopup(_prefabSingleButtonPopup);
            window.Init(description, ok, callback);
        }

        public void ShowDoublePopup(string description,
            string positive = "Accept", Action positiveCallback = null,
            string negative = "Cancel", Action negativeCallback = null)
        {
            _prefabDoubleButtonPopup ??= InAppContext.Prefabs.GetPopup<DoubleButtonPopup>();
            var window = ShowPopup(_prefabDoubleButtonPopup);
            window.Init(description, positive, positiveCallback, negative, negativeCallback);
        }
    }
}