using System;
using AppCore.UseCases;
using DG.Tweening;
using Domain.Enum;
using Presentation.Presenter;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class CalendarManager : MonoBehaviour
    {
        private const float TimeSwitchMode = 0.333f;

        [SerializeField] private PartsScheduleInDay schedulePrefab;
        [SerializeField] private CanvasGroup dayCanvas, weekCanvas;
        [SerializeField] private RectTransform calendar;
        
        private HistoryService _historyService;
        
        
        private CalendarType _calendarType = CalendarType.Invalid;

        private Sequence _modeSeq;
        
        private void Awake()
        {
            _historyService = InAppContext.Context.GetService<HistoryService>();
            SwitchMode(_historyService.TryGetHistory(HistoryType.PreviousCalendarType, out CalendarType type) ? type : CalendarType.OneDay);
        }
        
        private void SwitchMode(CalendarType type)
        {
            // 同じか、無効なやつに変えようとしてたらすぐ止める
            if (type is CalendarType.Invalid || _calendarType == type) return;

            // カレンダーのモードを変更する必要がある
            var modeWeek = IsCalendarModeWeek(type);
            if (_calendarType is CalendarType.Invalid)
            {
                _modeSeq.Kill();
                _modeSeq = DOTween.Sequence();
                if (modeWeek)
                {
                    dayCanvas.alpha = 0f;
                    dayCanvas.interactable = false;
                    weekCanvas.interactable = false;
                    _modeSeq.Append(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => weekCanvas.alpha = a))
                        .OnComplete(() => weekCanvas.interactable = true);
                }
                else
                {
                    weekCanvas.alpha = 0f;
                    weekCanvas.interactable = false;
                    dayCanvas.interactable = false;
                    _modeSeq.Append(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => dayCanvas.alpha = a))
                        .OnComplete(() => dayCanvas.interactable = true);
                }
            }
            else
            {
                if (modeWeek != IsCalendarModeWeek(_calendarType))
                {
                    _modeSeq.Kill();
                    _modeSeq = DOTween.Sequence();
                    if (modeWeek)
                    {
                        dayCanvas.interactable = false;
                        _modeSeq.Append(DOVirtual.Float(1f, 0f, TimeSwitchMode, a => dayCanvas.alpha = a))
                            .Join(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => weekCanvas.alpha = a))
                            .OnComplete(() => weekCanvas.interactable = true);
                    }
                    else
                    {
                        weekCanvas.interactable = false;
                        _modeSeq.Append(DOVirtual.Float(1f, 0f, TimeSwitchMode, a => weekCanvas.alpha = a))
                            .Join(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => dayCanvas.alpha = a))
                            .OnComplete(() => dayCanvas.interactable = true);
                    }
                }
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