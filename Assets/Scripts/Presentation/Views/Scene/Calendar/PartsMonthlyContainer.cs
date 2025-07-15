using System.Collections.Generic;
using Domain.Entity;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class PartsMonthlyContainer : MonoBehaviour
    {
        private const int Columns = 7, Rows = 6;
        private const float AnchorUnit = 1 / 3f;
        
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private PartsWeekDaysLabel prefabWeeklySeparator;
        [SerializeField] private RectTransform separatorParent;

        private readonly Dictionary<int, PartsWeekDaysLabel> _weeklySeparators = new();
        
        private CCDateOnly _targetDate;

        private void Awake()
        {
            for (var i = 0; i < Rows; i++)
            {
                var sep = Instantiate(prefabWeeklySeparator, separatorParent);
                sep.SetOffset(i);
                _weeklySeparators.Add(i, sep);
            }
        }
        
        public void Init(CCDateOnly day0)
        {
            _targetDate = day0;

            foreach (var (row, sep) in _weeklySeparators)
            {
                sep.Init(_targetDate.AddDays(row * Columns), row is 0);
            }
        }
        
        public void SetOffset(int offset)
        {
            rectTransform.anchorMin = new Vector2(AnchorUnit * (offset+1), 0f);
            rectTransform.anchorMax = new Vector2(AnchorUnit * (offset+2), 1f);
        }

        public (CCDateOnly startDate, CCDateOnly endDate) GetScheduleRange()
        {
            return (_targetDate, _targetDate.AddDays(Columns * Rows - 1));
        }

        public void PlaceSchedules(int offset, List<PartsScheduleInDay> schedules, bool instant)
        {
            var row = offset / Columns;
            var column = offset % Columns;
            var scheduleParent = _weeklySeparators[row];
            
            scheduleParent.PlaceSchedules(column, schedules, instant);
        }
    }
}