using System;
using AppCore.UseCases;
using Domain.Entity;
using Domain.Enum;
using Presentation.Presenter;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public class TaskCreationPopup : PopupWindow
    {
        private string _title = "";
        private string _description = "";

        public void SetTitle(string title)
        {
            _title = title;
        }

        public void SetDescription(string description)
        {
            _description = description;
        }

        public void CloseWindow(bool isCreateTask)
        {
            if (isCreateTask)
            {
                if (_title.Equals(""))
                {
                    Debug.Log("Title is needed OwO.");
                    return;
                }
                
                // DBにスケジュールを追加
                Schedule schedule = new Schedule(
                    0,
                    _title,
                    _description,
                    new ScheduleDuration(DateTime.Today, DateTime.Today.AddDays(1).AddMinutes(-1), false),
                    new SchedulePeriodic(SchedulePeriodicType.EveryWeek, 1));
                InAppContext.Context.GetService<ScheduleService>().CreateSchedule(schedule);
                
                Debug.Log("Created new task UwU");
            }
            else
            {
                Debug.Log("Canceled OwO");
            }
            
            CloseWindow();
        }
    }
}
