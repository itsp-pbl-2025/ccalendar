using System;
using System.Collections.Generic;
using Presentation.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Presentation.Views.Common
{
    [ExecuteInEditMode]
    public class AutoAspectCanvas : MonoBehaviour
    {
        private static readonly Vector2 PivotCenter = new(0.5f, 0.5f);
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private List<RectTransform> fullRectTransforms;
        [SerializeField] private List<RectTransform> safeRectTransforms;
        [SerializeField] private List<RectTransform> fixedRectTransforms;
        
        public Canvas Canvas => canvas;
        public RectTransform CanvasRect => canvasRect;
        
        private readonly HashSet<RectTransform> _fullRectTransforms = new();
        private readonly HashSet<RectTransform> _safeRectTransforms = new();
        private readonly HashSet<RectTransform> _fixedRectTransforms = new();
        
        private void Start()
        {
#if UNITY_EDITOR
            if (PrefabStageUtility.GetCurrentPrefabStage() == null) canvas.worldCamera = Camera.main;
#else
            canvas.worldCamera = Camera.main;
#endif

            foreach (var fullRect in fullRectTransforms)
            {
                _fullRectTransforms.Add(fullRect);
            }
            
            foreach (var safeRect in safeRectTransforms)
            {
                _safeRectTransforms.Add(safeRect);
            }

            foreach (var fixedRect in fixedRectTransforms)
            {
                _fixedRectTransforms.Add(fixedRect);
            }
            
            AdjustArea();
        }
        
        private void AdjustArea()
        {
            // DeviceSimulatorでスクリーンを切り替える瞬間に発生する場合がある 一度でも起きるとすべての子要素のYがNaNになる
            if (Screen.height == 0) return;

            var (safeMin, safeMax) = GetSafeAnchors();

            foreach (var fullRect in _fullRectTransforms)
            {
                fullRect.anchorMax = fullRect.anchorMin = PivotCenter;
                fullRect.sizeDelta = canvasRect.sizeDelta;
            }
            
            foreach (var safeRect in _safeRectTransforms)
            {
                safeRect.anchorMin = safeMin;
                safeRect.anchorMax = safeMax;
            }

            foreach (var fixedRect in _fixedRectTransforms)
            {
                fixedRect.pivot = Vector2.one / 2;
                fixedRect.sizeDelta = SceneSettings.FixedResolution;
            }
        }

        public void AddSafeRectTransform(RectTransform rctf)
        {
            if (rctf) _safeRectTransforms.Add(rctf);
            AdjustArea();
        }

        public bool RemoveSafeRectTransform(RectTransform rctf)
        {
            return _safeRectTransforms.Remove(rctf);
        }

        public void AddFullRectTransform(RectTransform rctf)
        {
            if (rctf) _fullRectTransforms.Add(rctf);
            AdjustArea();
        }

        public bool RemoveFullRectTransform(RectTransform rctf)
        {
           return  _fullRectTransforms.Remove(rctf);
        }

        public void AddFixedRectTransform(RectTransform rctf)
        {
            if (rctf) _fixedRectTransforms.Add(rctf);
            AdjustArea();
        }

        public bool RemoveFixedRectTransform(RectTransform rctf)
        {
            return _fixedRectTransforms.Remove(rctf);
        }

        private void Update()
        {
#if UNITY_EDITOR
            // Prefabモードでは実行しない
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
            
            AdjustArea();
#endif
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                AdjustArea();
            }
        }

        private static (Vector2 min, Vector2 max) GetSafeAnchors()
        {
            var wholeArea = new Vector2(Screen.width, Screen.height);
            var safeArea = Screen.safeArea;
            
#if PLATFORM_ANDROID && !UNITY_EDITOR
            // Androidの一部環境ではナビゲーションバーがsafeAreaに考慮されないので、下端をAndroidJavaObjectを使って計算する
            Vector2 safeMin;
            try
            {
                using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                using var helper = new AndroidJavaObject("itsp.teamc.safearea.SafeAreaHelper");
                var navBarHeight = helper.CallStatic<int>("getNavigationBarHeight", activity);

                safeMin = new Vector2(0, navBarHeight).Divide(wholeArea);
            }
            catch (Exception e)
            {
                Debug.LogWarning("NavBar height fetch failed: " + e);
                var density = Screen.dpi / 160f;
                var navBarPx = Mathf.RoundToInt(48f * density);
                safeMin = new Vector2(0f, navBarPx / (float)Screen.height);
            }
#else
            var safeMin = safeArea.position.Divide(wholeArea);
#endif
            var safeMax = (safeArea.position + safeArea.size).Divide(wholeArea);
            
            return (safeMin, safeMax);
        }
    }
}