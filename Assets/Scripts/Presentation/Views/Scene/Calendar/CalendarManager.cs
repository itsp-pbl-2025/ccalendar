using System.Collections.Generic;
using AppCore.UseCases;
using DG.Tweening;
using Domain.Entity;
using Domain.Enum;
using Presentation.Presenter;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class CalendarManager : MonoBehaviour
    {
        private const float TimeSwitchMode = 0.333f;
        private const float TimeSwitchType = 0.25f;

        [SerializeField] private PartsScheduleInDay schedulePrefab;
        [SerializeField] private CanvasGroup dayCanvas, weekCanvas;
        [SerializeField] private RectTransform dateIconContainer, dayContainer, weekContainer;
        [SerializeField] private RectTransform dateIconPageContent, dayPageContent, weekPageContent;
        [SerializeField] private PartsDayIconLabel dayFullIconPrefab, dayMiniIconPrefab;
        [SerializeField] private ImageRx daySeparatorPrefab;
        [SerializeField] private PartsWeekDaysLabel weekSeparatorPrefab;
        
        private HistoryService _historyService;
        
        private CalendarType _calendarType = CalendarType.Invalid;
        private CCDateOnly _targetDate;
        
        private List<PartsDayIconLabel> _dayIcons;
        private List<RectTransform> _daySeparators;
        private List<PartsWeekDaysLabel> _weekSeparators;
        
        private RectTransform _currentDateIconPage, _currentDayPage, _currentWeekPage;

        private Sequence _modeSeq, _typeSeq;
        
        private void Awake()
        {
            _targetDate = CCDateOnly.Today;
            _historyService = InAppContext.Context.GetService<HistoryService>();
            
            SwitchMode(_historyService.TryGetHistory(HistoryType.PreviousCalendarType, out CalendarType type) ? type : CalendarType.OneDay);
        }
        
        private void SwitchMode(CalendarType type)
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
                    }
                }

                switch (type)
                {
                    case CalendarType.OneDay:
                        break;
                    case CalendarType.ThreeDays:
                        break;
                    case CalendarType.OneWeek:
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