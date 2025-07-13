using System;
using System.Collections.Generic;
using Domain.Entity;
using Domain.Enum;
using Presentation.Views.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Popup
{
    public class PeriodicCreationPopup : PopupWindow
    {
        private enum PeriodicType
        {
            None,
            EveryDay,
            EveryWeek,
            EveryMonth,
            EveryYear,
        }
        
        [SerializeField] private RectTransform windowBaseRect;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private GameObject intervalSettings, weekdaySettings, monthSettings;
        [SerializeField] private List<GameObject> underEndDateObjects;
        [SerializeField] private ToggleWithLabel toggleEndDateLimited, toggleEndIndexLimited;
        [SerializeField] private ButtonWithLabel endDatePopupButton;
        [SerializeField] private TMP_InputField intervalInputField, endIndexInputField;
        
        private RectTransform _verticalLayoutRect;
        
        private SchedulePeriodic _periodic;
        private Action<SchedulePeriodic> _callback;

        public void Init(Action<SchedulePeriodic> onDefineCallback, SchedulePeriodic periodic = null)
        {
            _callback = onDefineCallback;
            _periodic = periodic ?? new SchedulePeriodic(SchedulePeriodicType.EveryDay, 1);
            
            ReloadInit();
        }

        private void ReloadInit()
        {
            _verticalLayoutRect = windowBaseRect.transform as RectTransform;

            ReloadAll();
        }

        private void ReloadAll()
        {
            if (_verticalLayoutRect)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutRect);

                if (_verticalLayoutRect.childCount == 0)
                {
                    Debug.LogWarning("AnyPeriodicPopup doesn't have any content.");
                    return;
                }
            
                var lastChildRect = _verticalLayoutRect.GetChild(_verticalLayoutRect.childCount-1) as RectTransform;

                if (lastChildRect == null)
                {
                    Debug.LogWarning("AnyPeriodicPopup's Last Child is not RectTransform-ed-Object.");
                    return;
                }
            
                windowBaseRect.sizeDelta = new Vector2(windowBaseRect.sizeDelta.x,
                    Mathf.Abs(lastChildRect.anchoredPosition.y) + lastChildRect.sizeDelta.y);
            }
        }
    }
}