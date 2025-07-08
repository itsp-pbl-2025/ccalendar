using System;
using AppCore.UseCases;
using Domain.Entity;
using Presentation.Presenter;
using TMPro;
using UnityEngine;

namespace Presentation.Views.Popup.Task
{
    public class TaskCreationPopup : PopupWindow
    {
        [SerializeField] private TextMeshProUGUI deadlineText;
        [SerializeField] private TextMeshProUGUI priorityText;

        private string _title = "";
        private string _description = "";
        private int _priority;
        private CCDateOnly _deadlineDate;
        private CCTimeOnly _deadlineTime;

        public void Awake()
        {
            _deadlineDate = CCDateOnly.Today.AddDays(1); // set current time
            _deadlineTime = CCTimeOnly.Now;
            SetDeadlineText();
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        public void SetDescription(string description)
        {
            _description = description;
        }

        public void SetPriority(float priority)
        {
            _priority = Math.Min((int) Math.Floor(priority * 5.0f) + 1, 5); // 1 to 5
            priorityText.text = $"優先度: {_priority}";
        }
        
        public void OpenDatePopup()
        {
            var popup = InAppContext.Prefabs.GetPopup<DateOnlyPopup>();
            var window = PopupManager.Instance.ShowPopup(popup);
            window.Init(x =>
            {
                _deadlineDate = x;
                SetDeadlineText();
            });
        }
        
        public void OpenTimePopup()
        {
            var popup = InAppContext.Prefabs.GetPopup<TimeOnlyPopup>();
            var window = PopupManager.Instance.ShowPopup(popup);
            window.Init(x =>
            {
                _deadlineTime = x;
                SetDeadlineText();
            });
        }

        private void OpenDateOnlyPopup(Action<CCDateOnly> onDateTimeDefined)
        {
            var popup = InAppContext.Prefabs.GetPopup<DateOnlyPopup>();
            var window = PopupManager.Instance.ShowPopup(popup);
            window.Init(onDateTimeDefined);
        }
        
        private void SetDeadlineText()
        {
            deadlineText.text = $"締め切り: {_deadlineTime.WithDate(_deadlineDate):yyyy年MM月dd日 hh:mm:ss}";
        }

        public void CloseWindow(bool isCreateTask)
        {
            if (isCreateTask)
            {
                if (_title.Equals(""))
                {
                    Debug.LogWarning("Title is needed 🧟");
                    return;
                }
                
                CCTask task = new (0, _title, _description, _priority, new CCDateTime(_deadlineDate, _deadlineTime));
                InAppContext.Context.GetService<TaskService>().CreateTask(task);
                
                Debug.Log("Created new task 🦊");
            }
            else
            {
                Debug.Log("Canceled 🐬");
            }
            
            CloseWindow();
        }
    }
}
