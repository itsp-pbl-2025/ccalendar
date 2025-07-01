#if UNITY_EDITOR
using Presentation.Resources;
using UnityEditor;

namespace Presentation.Utilities
{
    public static class AssetInEditor
    {
        private const string PathThemeDefault = "Assets/Prefabs/Scriptable/Theme/ThemeDefault.asset";
        
        private static ThemePalette _theme;
        
        public static ThemePalette Theme
        {
            get
            {
                if (_theme == null)
                {
                    var colorTheme = AssetDatabase.LoadAssetAtPath<ColorTheme>(PathThemeDefault);
                    _theme = new ThemePalette(colorTheme);
                }
                return _theme;
            }
        }
    }
}
#endif