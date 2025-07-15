using System;
using System.Collections.Generic;
using Presentation.Views.Extensions;
using Presentation.Views.Popup;
using UnityEngine;

namespace Presentation.Views.Scene.Sample
{
    public class SampleRelationExplainer : MonoBehaviour
    {
        private enum NamespaceType
        {
			Domain,
            AppCore,
            Infrastructure,
            Presentation,
            Test,
            Runner
        }
        
        [Serializable]
        private class RectTypePair
        {
            public NamespaceType type;
            public RectTransform rect;
        }
        
        [SerializeField] private RectFigurePopup rectFigurePrefab;
        [SerializeField] private List<RectTypePair> relationRectPrefabs;

        [EnumAction(typeof(NamespaceType))]
        public void ShowRelationRect(int intType)
        {
            var type = (NamespaceType)intType;

            foreach (var pair in relationRectPrefabs)
            {
                if (pair.type == type)
                {
                    var window = PopupManager.Instance.ShowPopup(rectFigurePrefab);
                    window.Init(pair.rect);
                    return;
                }
            }
            
            Debug.LogWarning($"No relation rect found for type: {type}");
        }
    }
}