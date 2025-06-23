using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Presentation.Views.Common
{
    [ExecuteInEditMode]
    public class AutoAspectCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private List<RectTransform> fullRectBoxColliders;
        [SerializeField] private List<RectTransform> safeRectTransforms;
        [SerializeField] private List<RectTransform> fixedRectTransforms;
        
        public Canvas Canvas => canvas;
        public RectTransform CanvasRect => canvasRect;
        public CanvasGroup CanvasGroup => canvasGroup;
        
        private readonly HashSet<RectTransform> _fullRectTransform = new();
        private readonly HashSet<RectTransform> _safeRectTransforms = new();
        private readonly HashSet<RectTransform> _fixedRectTransforms = new();
        
        private void Start()
        {
#if UNITY_EDITOR
            if (PrefabStageUtility.GetCurrentPrefabStage() == null) canvas.worldCamera = Camera.main;
#else
            canvas.worldCamera = Camera.main;
#endif
            
            foreach (var safeRect in safeRectTransforms)
            {
                _safeRectTransforms.Add(safeRect);
            }

            foreach (var fullRect in fullRectBoxColliders)
            {
                _fullRectTransform.Add(fullRect);
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
            
            // anchorMaxのY(上端)をsafeAreaに合わせて調整する 上以外の要素は今のところ考慮しない(下端の丸みなど)
            var screen = Screen.safeArea;
            var anchorMax = new Vector2(1, (screen.position + screen.size).y / Screen.height);

            foreach (var fullRect in _fullRectTransform)
            {
                // TODO: どの子要素でも常に背景と同じ大きさになるような調整方法に変更できるとUIの自由度が高まる
                fullRect.anchorMax = Vector2.one;
            }
            
            foreach (var safeRect in _safeRectTransforms)
            {
                safeRect.anchorMax = anchorMax;
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
            if (rctf) _fullRectTransform.Add(rctf);
            AdjustArea();
        }

        public bool RemoveFullRectTransform(RectTransform rctf)
        {
           return  _fullRectTransform.Remove(rctf);
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
    }
}