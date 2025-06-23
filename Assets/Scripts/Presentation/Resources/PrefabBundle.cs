using System;
using System.Collections.Generic;
using Presentation.Views.Popup;

namespace Presentation.Resources
{
    public class PrefabBundle
    {
        private readonly List<PopupWindow> _popupPrefabs;
        private readonly List<ColorTheme> _builtinThemes;
        
        public PrefabBundle(PrefabDictionary prefabDict)
        {
            _popupPrefabs = prefabDict.PopupPrefabs;
            _builtinThemes = prefabDict.BuiltinThemes;
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

        public List<ColorTheme> GetThemeAll()
        {
            return new List<ColorTheme>(_builtinThemes);
        }

        public ColorTheme GetThemeByName(string name)
        {
            foreach (var theme in _builtinThemes)
            {
                if (theme.name == name)
                {
                    return theme;
                }
            }
            return _builtinThemes[0];
        }
    }
}