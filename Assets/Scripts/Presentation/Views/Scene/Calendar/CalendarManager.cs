using System.Collections.Generic;
using AppCore.UseCases;
using DG.Tweening;
using Domain.Entity;
using Domain.Enum;
using Presentation.Presenter;
using Presentation.Views.Extensions;
using Presentation.Views.Popup;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class CalendarManager : MonoBehaviour
    {
        private const float TimeSwitchMode = 1/3f;
        private const float TimeSwitchType = 0.25f;

        private const float WidthDayContainer = 980f;
        private const float ThresholdPageSwitch = 1/3f;

        [SerializeField] private CalendarSidebarPopup sidebarPopupPrefab;
        [SerializeField] private PartsScheduleInDay schedulePrefab;
        [SerializeField] private CanvasGroup dayCanvas, weekCanvas;
        [SerializeField] private ScalableScrollRect dayScrollRect;
        [SerializeField] private RectTransform dayContainer, weekContainer;
        [SerializeField] private RectTransform dayPageContent, weekPageContent;
        [SerializeField] private PartsDayIconLabel dayFullIconPrefab, dayMiniIconPrefab;
        [SerializeField] private RectTransform dateIconContent;
        [SerializeField] private ImageRx daySeparatorPrefab;
        [SerializeField] private PartsWeekDaysLabel weekSeparatorPrefab;
        
        private HistoryService _historyService;
        
        private CalendarType _calendarType = CalendarType.Invalid;
        public CCDateOnly CurrentTargetDate => _targetDate;
        private CCDateOnly _targetDate;
        
        private readonly Dictionary<int, PartsDayIconLabel> _dayIcons = new();
        private List<PartsWeekDaysLabel> _weekSeparators;
        
        private readonly Dictionary<int, RectTransform> _dayPageContents = new();
        
        private RectTransform _currentDateIconPage, _currentDayPage, _currentWeekPage;

        private Sequence _modeSeq, _typeSeq;
        
        private void Awake()
        {
            _targetDate = CCDateOnly.Today;
            _historyService = InAppContext.Context.GetService<HistoryService>();
            
            SwitchMode(_historyService.TryGetHistory(HistoryType.PreviousCalendarType, out CalendarType type) ? type : CalendarType.OneDay);
        }

        public void ShowSidebar()
        {
            var window = PopupManager.Instance.ShowPopup(sidebarPopupPrefab);
            window.Init(this);
        }
        
        public void SwitchMode(CalendarType type)
        {
            // 同じか、無効なやつに変えようとしてたらすぐ止める
            if (type is CalendarType.Invalid || _calendarType == type) return;

            var nextModeIsWeek = IsCalendarModeWeek(type);
            var fromInvalid = _calendarType is CalendarType.Invalid;
            var initAlpha = 1f;
            
            _typeSeq?.Complete();
            _typeSeq = DOTween.Sequence().AppendInterval(TimeSwitchType);
            
            // 初期状態なら、全部一回透明にしちゃう
            if (fromInvalid)
            {
                initAlpha = 0f;
                dayCanvas.alpha = 0f;
                weekCanvas.alpha = 0f;
                dayCanvas.interactable = false;
                weekCanvas.interactable = false;
            }
            
            // カレンダーのモードを変更する必要があるなら、まずモードを調整して、今のモードの諸々をしまう
            if (fromInvalid || nextModeIsWeek != IsCalendarModeWeek(_calendarType))
            {
                _modeSeq?.Complete();
                _modeSeq = DOTween.Sequence();
                if (nextModeIsWeek)
                {
                    dayCanvas.interactable = false;
                    _modeSeq.Append(DOVirtual.Float(initAlpha, 0f, TimeSwitchMode, a => dayCanvas.alpha = a))
                        .Join(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => weekCanvas.alpha = a))
                        .OnComplete(() => weekCanvas.interactable = true);

                    // TODO: dayCanvasの諸々をしまう
                    if (!fromInvalid)
                    {
                    }

                    // TODO: weekCanvasの準備をする
                    {
                    }
                }
                else
                {
                    weekCanvas.interactable = false;
                    _modeSeq.Append(DOVirtual.Float(initAlpha, 0f, TimeSwitchMode, a => weekCanvas.alpha = a))
                        .Join(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => dayCanvas.alpha = a))
                        .OnComplete(() => dayCanvas.interactable = true);

                    // TODO: weekCanvasの諸々をしまう
                    if (!fromInvalid)
                    {
                    }
                    
                    // TODO: dayCanvasの準備をする
                    {
                        InitPageAll(type);
                    }
                }

                switch (type)
                {
                    case CalendarType.OneDay:
                        dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, StepDateIcon);
                        break;
                    case CalendarType.ThreeDays:
                        dayScrollRect.SetStepPageSettings(WidthDayContainer/3, ThresholdPageSwitch, StepDateIcon);
                        break;
                    case CalendarType.OneWeek:
                        dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, StepDateIcon);
                        break;
                    case CalendarType.ThreeWeeks:
                        break;
                    case CalendarType.OneMonth:
                        break;
                }
            }
            // ThreeWeeks <-> OneMonthの切り替え処理
            else if (nextModeIsWeek)
            {
                
            }
            // OneDay, ThreeDays, OneWeekの切り替え処理
            else
            {
                
            }

            _calendarType = type;
        }

        private void InitPageAll(CalendarType type)
        {
            switch (type)
            {
                case CalendarType.OneDay:
                    for (var offset = -1; offset <= 1; offset++)
                    {
                        var dayIcon = Instantiate(dayFullIconPrefab, dateIconContent);
                        dayIcon.Init(_targetDate.AddDays(offset));
                        dayIcon.SetAnchorX(1/3f * offset + 1/2f);
                        _dayIcons.Add(offset, dayIcon);

                        var dayPage = Instantiate(dayPageContent, dayContainer);
                        dayPage.gameObject.SetActive(true);
                        dayPage.anchorMin = new Vector2(1 / 3f * (offset + 1), 0f);
                        dayPage.anchorMax = new Vector2(1 / 3f * (offset + 2), 1f);
                        _dayPageContents.Add(offset, dayPage);
                    }
                    break;
                case CalendarType.ThreeDays:
                    for (var offset = -3; offset <= 6; offset++)
                    {
                        var dayIcon = Instantiate(dayMiniIconPrefab, dateIconContent);
                        dayIcon.Init(_targetDate.AddDays(offset));
                        dayIcon.SetAnchorX(1/9f * offset + 1/2f);
                        _dayIcons.Add(offset, dayIcon);

                        var dayPage = Instantiate(dayPageContent, dayContainer);
                        dayPage.gameObject.SetActive(true);
                        dayPage.anchorMin = new Vector2(1 / 9f * (offset + 3), 0f);
                        dayPage.anchorMax = new Vector2(1 / 9f * (offset + 4), 1f);
                        _dayPageContents.Add(offset, dayPage);
                    }
                    break;
                case CalendarType.OneWeek:
                    for (var offset = -7; offset <= 14; offset++)
                    {
                        var dayIcon = Instantiate(dayMiniIconPrefab, dateIconContent);
                        dayIcon.Init(_targetDate.AddDays(offset));
                        dayIcon.SetAnchorX(1/21f * offset + 1/2f);
                        _dayIcons.Add(offset, dayIcon);

                        var dayPage = Instantiate(dayPageContent, dayContainer);
                        dayPage.gameObject.SetActive(true);
                        dayPage.anchorMin = new Vector2(1 / 21f * (offset + 7), 0f);
                        dayPage.anchorMax = new Vector2(1 / 21f * (offset + 8), 1f);
                        _dayPageContents.Add(offset, dayPage);
                    }
                    break;
                case CalendarType.ThreeWeeks:
                    break;
                case CalendarType.OneMonth:
                    break;
            }
        }

        private void StepDateIcon(int stepPages)
        {
            if (stepPages is 0) return;
            
            // 日付アイコンを移動させる
            switch (_calendarType)
            {
                case CalendarType.OneDay:
                case CalendarType.ThreeDays:
                case CalendarType.OneWeek:
                {
                    // TODO: _dayPageContentsの中身をちゃんと移動させるから、OneDay+ThreeDaysとOneWeekを別で処理
                    _targetDate = _targetDate.AddDays(stepPages);
                    foreach (var (offset, icon) in _dayIcons)
                    {
                        icon.Init(_targetDate.AddDays(offset));
                    }

                    break;
                }
                case CalendarType.ThreeWeeks:
                    break;
                case CalendarType.OneMonth:
                    break;
            }
        }

        private static bool IsCalendarModeWeek(CalendarType type)
        {
            return type is CalendarType.ThreeWeeks or CalendarType.OneMonth;
        }

        private void OnDisable()
        {
            _historyService.UpdateHistory(HistoryType.PreviousCalendarType, _calendarType);
        }
    }
}