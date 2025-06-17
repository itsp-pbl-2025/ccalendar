using System;
using System.Collections.Generic;
using Presentation.Views.Popup;

namespace Presentation.Resources
{
    public class PrefabBundle
    {
        private readonly List<PopupWindow> _popupPrefabs;
        
        public PrefabBundle(PrefabDictionary prefabDict)
        {
            _popupPrefabs = prefabDict.PopupPrefabs;
        }
        
        public T GetPopup<T>(string name = "") where T : PopupWindow
        {
            var findByName = name != "";
            foreach (var prefab in _popupPrefabs)
            {
                if (prefab is T popup && (!findByName || popup.gameObject.name == name))
                {
                    return popup;
                }
            }
            throw new InvalidOperationException($"No prefab found for type {typeof(T)}");
        }
    }
}