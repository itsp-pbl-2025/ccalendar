using System;
using Domain.Entity;
using Presentation.Presenter;
using Presentation.Views.Popup;
using TMPro;
using UnityEngine;

namespace Presentation.Views.Private
{
    public class DhiraSceneManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        private void Start()
        {
            OnDateSelected(default);
            OnTimeSelected(default);
        }

        public void OpenDateOnlyPopup()
        {
            var popup = InAppContext.Prefabs.GetPopup<DateOnlyPopup>();
            var window = PopupManager.Instance.ShowPopup(popup);
            window.Init(OnDateSelected);
        }

        public void OpenTimeOnlyPopup()
        {
            var popup = InAppContext.Prefabs.GetPopup<TimeOnlyPopup>();
            var window = PopupManager.Instance.ShowPopup(popup);
            window.Init(OnTimeSelected);
        }

        private void OnDateSelected(CCDateOnly dt)
        {
            dateText.text = dt.IsDefault() ? "" : dt.ToDateTime().ToString("yyyy年MM月dd日");
        }

        private void OnTimeSelected(CCTimeOnly to)
        {
            timeText.text = $"{to.Hour.Value:D2}:{to.Minute.Value:D2}:{to.Second.Value:D2}";
        }
    }
}