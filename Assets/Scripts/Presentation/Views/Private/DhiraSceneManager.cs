using System;
using Presentation.Presenter;
using Presentation.Views.Popup;
using TMPro;
using UnityEngine;

namespace Presentation.Views.Private
{
    public class DhiraSceneManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dateText;
        
        private void Start()
        {
            OnDateSelected(default);
        }

        public void OpenDateOnlyPopup()
        {
            var popup = InAppContext.Prefabs.GetPopup<DateOnlyPopup>();
            var window = PopupManager.Instance.ShowPopup(popup);
            window.Init(OnDateSelected);
        }

        private void OnDateSelected(DateTime dt)
        {
            dateText.text = dt == default ? "" : dt.ToString("yyyy年MM月dd日");
        }
    }
}