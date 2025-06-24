using System.Collections.Generic;
using Presentation.Views.Popup;
using UnityEngine;

namespace Presentation.Resources
{
    [CreateAssetMenu(fileName = "PrefabDictionary", menuName = "Context/PrefabDictionary", order = 1)]
    public class PrefabDictionary : ScriptableObject
    {
        [SerializeField] private List<PopupWindow> popupPrefabs;
        [SerializeField] private List<ColorTheme> themes;

        public List<PopupWindow> PopupPrefabs => new(popupPrefabs);
        public List<ColorTheme> BuiltinThemes => new(themes);
    }
}