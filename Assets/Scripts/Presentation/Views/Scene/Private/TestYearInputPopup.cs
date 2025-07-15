using Domain.Entity;
using Presentation.Presenter;
using Presentation.Views.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Scene.Private
{
    public class TestYearInputPopup : MonoBehaviour
    {
        [SerializeField] private Button openButton;

        public void OpenPopup()
        {
            var popup = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<YearInputPopup>());
            popup.Init(OnYearSelected, CCDateOnly.Today.Year.Value);
        }

        private void OnYearSelected(int selectedYear)
        {
            Debug.Log($"✅ 年が選択されました: {selectedYear}");
        }
    }
}