using System;
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
        private const float ThresholdPageSwitch = 1/5f;

        [SerializeField] private CalendarSidebarPopup sidebarPopupPrefab;
        [SerializeField] private PartsScheduleInDay schedulePrefab;
        [SerializeField] private CanvasGroup dayCanvas, weekCanvas;
        [SerializeField] private ScalableScrollRect dayScrollRect;
        [SerializeField] private RectTransform dayContainer, weekContainer, animationContainer;
        [SerializeField] private RectTransform dayPageContent, weekPageContent;
        [SerializeField] private PartsDayIconLabel dayFullIconPrefab, dayMiniIconPrefab;
        [SerializeField] private RectTransform dateIconContent;
        [SerializeField] private ImageRx daySeparatorPrefab;
        [SerializeField] private PartsWeekDaysLabel weekSeparatorPrefab;
        
        private ScheduleService _scheduleService;
        private HistoryService _historyService;
        
        private CalendarType _calendarType = CalendarType.Invalid;
        public CCDateOnly CurrentTargetDate => _targetDate;
        private CCDateOnly _targetDate;
        
        private readonly Dictionary<int, PartsDayIconLabel> _dayIcons = new();
        private List<PartsWeekDaysLabel> _weekSeparators;
        
        private readonly Dictionary<int, RectTransform> _dayPageContents = new();
        private readonly Dictionary<CCDateOnly, List<PartsScheduleInDay>> _scheduleInDays = new();
        
        private RectTransform _currentDateIconPage, _currentDayPage, _currentWeekPage;

        private Sequence _modeSeq, _typeSeq;
        
        private void Awake()
        {
            _targetDate = CCDateOnly.Today;
            _scheduleService = InAppContext.Context.GetService<ScheduleService>();
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
            var prevDate = _targetDate;
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
            
            // どのくらいスクロールしたらページを隣にするか？
            switch (type)
            {
                case CalendarType.OneDay:
                    dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, StepDateIcon);
                    break;
                case CalendarType.ThreeDays:
                    dayScrollRect.SetStepPageSettings(WidthDayContainer/3, ThresholdPageSwitch, StepDateIcon);
                    break;
                case CalendarType.OneWeek:
                    _targetDate = _targetDate.AddDays(-(int)_targetDate.ToDateTime().DayOfWeek);
                    dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, StepDateIcon);
                    break;
                case CalendarType.ThreeWeeks:
                    _targetDate = _targetDate.AddDays(-(int)_targetDate.ToDateTime().DayOfWeek);
                    dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, null);
                    break;
                case CalendarType.OneMonth:
                    var firstWeek = _targetDate.AddDays(-_targetDate.ToDateTime().Day);
                    _targetDate = _targetDate.AddDays(-(int)firstWeek.ToDateTime().DayOfWeek);
                    dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, null);
                    break;
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
            }
            // ThreeWeeks <-> OneMonthの切り替え処理
            else if (nextModeIsWeek)
            {
                
            }
            // OneDay, ThreeDays, OneWeekの切り替え処理
            else
            {
                switch (type)
                {
                    case CalendarType.OneDay:
                        for (var offset = -7; offset < 14; offset++)
                        {
                            if (!_dayPageContents.TryGetValue(offset, out var dayPage)) continue;

                            if (offset is >= -1 and < 2)
                            {
                                dayPage.anchorMin = new Vector2(1/3f * (offset + 1), 0f);
                                dayPage.anchorMax = new Vector2(1/3f * (offset + 2), 1f);
                                RefreshTypedDayIcon(offset, _targetDate, CalendarType.OneDay);
                            }
                            else
                            {
                                RemoveDayContainer(offset, _targetDate);
                            }
                        }
                        break;
                    case CalendarType.ThreeDays:
                        if (_calendarType is CalendarType.OneDay)
                        {
                            for (var offset = -3; offset < 6; offset++)
                            {
                                if (_dayPageContents.TryGetValue(offset, out var dayPage))
                                {
                                    dayPage.anchorMin = new Vector2(1/9f * (offset + 3), 0f);
                                    dayPage.anchorMax = new Vector2(1/9f * (offset + 4), 1f);
                                    RefreshTypedDayIcon(offset, _targetDate, CalendarType.ThreeDays);
                                }
                                else
                                {
                                    CreateSingleDayContainer(offset, _targetDate, CalendarType.ThreeDays);
                                }
                            }
                        }
                        else if (_calendarType is CalendarType.OneWeek)
                        {
                            for (var offset = -7; offset < 14; offset++)
                            {
                                if (!_dayPageContents.TryGetValue(offset, out var dayPage)) continue;

                                if (offset is >= -3 and < 6)
                                {
                                    dayPage.anchorMin = new Vector2(1/9f * (offset + 3), 0f);
                                    dayPage.anchorMax = new Vector2(1/9f * (offset + 4), 1f);
                                    RefreshTypedDayIcon(offset, _targetDate, CalendarType.ThreeWeeks);
                                }
                                else
                                {
                                    RemoveDayContainer(offset, _targetDate);
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(_calendarType), _calendarType, null);
                        }
                        break;
                    case CalendarType.OneWeek:
                        for (var offset = -7; offset < 14; offset++)
                        {
                            if (_dayPageContents.TryGetValue(offset, out var dayPage))
                            {
                                dayPage.anchorMin = new Vector2(1/21f * (offset + 7), 0f);
                                dayPage.anchorMax = new Vector2(1/21f * (offset + 8), 1f);
                                RefreshTypedDayIcon(offset, _targetDate, CalendarType.OneWeek);
                            }
                            else
                            {
                                CreateSingleDayContainer(offset, _targetDate, CalendarType.OneWeek);
                            }
                        }
                        break;
                    case CalendarType.ThreeWeeks: case CalendarType.OneMonth: default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }

            _calendarType = type;
        }

        private void InitPageAll(CalendarType type)
        {
            switch (type)
            {
                case CalendarType.OneDay:
                case CalendarType.ThreeDays:
                case CalendarType.OneWeek:
                    var containerInPage = ElementsInCalendar(type);
                    for (var offset = -containerInPage; offset < 2 * containerInPage; offset++)
                    {
                        CreateSingleDayContainer(offset, _targetDate, type);
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
                {
                    _targetDate = _targetDate.AddDays(stepPages);
                    foreach (var (offset, icon) in _dayIcons)
                    {
                        icon.Init(_targetDate.AddDays(offset));
                    }

                    switch (stepPages)
                    {
                        case > 2 or < -2:
                            foreach (var (_, inDay) in _scheduleInDays)
                            {
                                foreach (var node in inDay)
                                {
                                    node.SetParent(animationContainer);
                                    node.Decay();
                                }
                            }
                            _scheduleInDays.Clear();
                            
                            for (var offset = -1; offset < 2; offset++)
                            {
                                FillScheduleInDayContainer(offset, _targetDate);
                            }
                            break;
                        case > 0:
                            for (var offset = -1-stepPages; offset < 2; offset++)
                            {
                                if (offset < -1)
                                {
                                    CleanupDayContainer(offset, _targetDate);
                                }
                                else if (offset <= 1-stepPages)
                                {
                                    MoveAllScheduleToDayContainer(offset, _targetDate);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset, _targetDate);
                                }
                            }
                            break;
                        case < 0:
                            for (var offset = 1-stepPages; offset >= -1; offset--)
                            {
                                if (offset > 1)
                                {
                                    CleanupDayContainer(offset, _targetDate);
                                }
                                else if (offset >= -1-stepPages)
                                {
                                    MoveAllScheduleToDayContainer(offset, _targetDate);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset, _targetDate);
                                }
                            }
                            break;
                    }

                    break;
                }
                case CalendarType.ThreeDays:
                {
                    _targetDate = _targetDate.AddDays(stepPages);
                    foreach (var (offset, icon) in _dayIcons)
                    {
                        icon.Init(_targetDate.AddDays(offset));
                    }

                    switch (stepPages)
                    {
                        case > 8 or < -8:
                            foreach (var (_, inDay) in _scheduleInDays)
                            {
                                foreach (var node in inDay)
                                {
                                    node.SetParent(animationContainer);
                                    node.Decay();
                                }
                            }
                            _scheduleInDays.Clear();
                            
                            for (var offset = -3; offset < 6; offset++)
                            {
                                FillScheduleInDayContainer(offset, _targetDate);
                            }
                            break;
                        case > 0:
                            for (var offset = -3-stepPages; offset < 6; offset++)
                            {
                                if (offset < -3)
                                {
                                    CleanupDayContainer(offset, _targetDate);
                                }
                                else if (offset <= 6-stepPages)
                                {
                                    MoveAllScheduleToDayContainer(offset, _targetDate);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset, _targetDate);
                                }
                            }
                            break;
                        case < 0:
                            for (var offset = 5-stepPages; offset >= -3; offset--)
                            {
                                if (offset >= 6)
                                {
                                    CleanupDayContainer(offset, _targetDate);
                                }
                                else if (offset > -3-stepPages)
                                {
                                    MoveAllScheduleToDayContainer(offset, _targetDate);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset, _targetDate);
                                }
                            }
                            break;
                    }
                    
                    break;
                }
                case CalendarType.OneWeek:
                {
                    _targetDate = _targetDate.AddDays(stepPages * 7);
                    foreach (var (offset, icon) in _dayIcons)
                    {
                        icon.Init(_targetDate.AddDays(offset));
                    }
                    
                    switch (stepPages)
                    {
                        case > 2 or < -2:
                            foreach (var (_, inDay) in _scheduleInDays)
                            {
                                foreach (var node in inDay)
                                {
                                    node.SetParent(animationContainer);
                                    node.Decay();
                                }
                            }
                            _scheduleInDays.Clear();
                            
                            for (var offset = -7; offset < 14; offset++)
                            {
                                FillScheduleInDayContainer(offset, _targetDate);
                            }
                            break;
                        case > 0:
                            for (var offset = -1-stepPages; offset < 2; offset++)
                            {
                                if (offset < -1)
                                {
                                    for (var j=0; j<7; j++) CleanupDayContainer(offset*7+j, _targetDate);
                                }
                                else if (offset <= 1-stepPages)
                                {
                                    for (var j=0; j<7; j++) MoveAllScheduleToDayContainer(offset*7+j, _targetDate);
                                }
                                else
                                {
                                    for (var j=0; j<7; j++) FillScheduleInDayContainer(offset*7+j, _targetDate);
                                }
                            }
                            break;
                        case < 0:
                            for (var offset = 1-stepPages; offset >= -1; offset--)
                            {
                                if (offset > 1)
                                {
                                    for (var j=0; j<7; j++) CleanupDayContainer(offset*7+j, _targetDate);
                                }
                                else if (offset >= -1-stepPages)
                                {
                                    for (var j=0; j<7; j++) MoveAllScheduleToDayContainer(offset*7+j, _targetDate);
                                }
                                else
                                {
                                    for (var j=0; j<7; j++) FillScheduleInDayContainer(offset*7+j, _targetDate);
                                }
                            }
                            break;
                    }

                    break;
                }
                case CalendarType.ThreeWeeks:
                    break;
                case CalendarType.OneMonth:
                    break;
            }
        }

        /// <summary>
        /// ある日付から特定のoffset離れた位置の日付アイコンを作成、すでにあれば作り直す。
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        /// <param name="targetDate">作成の基準となる日付</param>
        /// <param name="calendarType">カレンダーの状態</param>
        private void RefreshTypedDayIcon(int offset, CCDateOnly targetDate, CalendarType calendarType)
        {
            var containersInPage = ElementsInCalendar(calendarType);
            if (_dayIcons.Remove(offset, out var removeIcon)) Destroy(removeIcon.gameObject);
            
            var widthInAnchor = 1f / (3 * containersInPage);
            var dayIcon = Instantiate(calendarType is CalendarType.OneDay ? dayFullIconPrefab : dayMiniIconPrefab, dateIconContent);
            dayIcon.Init(targetDate.AddDays(offset));
            dayIcon.SetAnchorX(1/3f + widthInAnchor/2 * (1 + 2 * offset));
            _dayIcons.Add(offset, dayIcon);
        }

        /// <summary>
        /// 日カレンダーにおける1日を適切な位置に新規作成する。日付アイコンも作る。
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        /// <param name="targetDate">作成の基準となる日付</param>
        /// <param name="calendarType">カレンダーの状態</param>
        private void CreateSingleDayContainer(int offset, CCDateOnly targetDate, CalendarType calendarType)
        {
            var containersInPage = ElementsInCalendar(calendarType);
            var widthInAnchor = 1f / (3 * containersInPage);
            var day = targetDate.AddDays(offset);
            RefreshTypedDayIcon(offset, targetDate, calendarType);

            var dayPage = Instantiate(dayPageContent, dayContainer);
            dayPage.gameObject.SetActive(true);
            dayPage.anchorMin = new Vector2(widthInAnchor * (offset + containersInPage), 0f);
            dayPage.anchorMax = new Vector2(widthInAnchor * (offset + containersInPage + 1), 1f);
            _dayPageContents.Add(offset, dayPage);

            CleanupDayContainer(offset, targetDate);
            var inDay = new List<PartsScheduleInDay>();
            foreach (var schedule in _scheduleService.GetSchedulesInDuration(new ScheduleDuration(day)))
            {
                var node = Instantiate(schedulePrefab, dayPage);
                node.Init(schedule, day);
                node.Transform(0f, 1f);
                inDay.Add(node);
            }
            _scheduleInDays.Add(day, inDay);
        }

        /// <summary>
        /// 特定の日付におけるスケジュールを再読み込みし、スケジュールオブジェクトを作成する。
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        /// <param name="targetDate">更新の基準となる日付</param>
        private void FillScheduleInDayContainer(int offset, CCDateOnly targetDate)
        {
            CleanupDayContainer(offset, targetDate);
            var day = targetDate.AddDays(offset);
            // TODO: ここoffsetの役割が被ってるから、ここのoffsetは本来_targetDateからの距離じゃなきゃいけなくて、制限がある
            var dayPage = _dayPageContents[offset];
            
            CleanupDayContainer(offset, targetDate);
            var inDay = new List<PartsScheduleInDay>();
            foreach (var schedule in _scheduleService.GetSchedulesInDuration(new ScheduleDuration(day)))
            {
                var node = Instantiate(schedulePrefab, dayPage);
                node.Init(schedule, day);
                node.Transform(0f, 1f);
                inDay.Add(node);
            }
            _scheduleInDays.Add(day, inDay);
        }

        /// <summary>
        /// 特定の日付におけるスケジュールオブジェクトを削除する
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        /// <param name="targetDate">削除の基準となる日付</param>
        private void CleanupDayContainer(int offset, CCDateOnly targetDate)
        {
            var day = targetDate.AddDays(offset);
            if (_scheduleInDays.Remove(day, out var inDay))
            {
                foreach (var node in inDay)
                {
                    node.SetParent(animationContainer);
                    node.Decay();
                }
            }
        }
        
        /// <summary>
        /// 特定の日付コンテナを削除する
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        /// <param name="targetDate">削除の基準となる日付</param>
        private void RemoveDayContainer(int offset, CCDateOnly targetDate)
        {
            var day = targetDate.AddDays(offset);
            if (_dayIcons.Remove(offset, out var dayIcon)) Destroy(dayIcon.gameObject);
            if (_scheduleInDays.Remove(day, out var inDay))
            {
                foreach (var node in inDay)
                {
                    node.SetParent(animationContainer);
                    node.Decay();
                }
            }
            if (_dayPageContents.Remove(offset, out var dayPage)) Destroy(dayPage.gameObject);
        }

        /// <summary>
        /// ページ移動に伴ってズレたスケジュールオブジェクトを正しい親に帰属させる
        /// </summary>
        /// <param name="to"></param>
        /// <param name="targetDate"></param>
        private void MoveAllScheduleToDayContainer(int to, CCDateOnly targetDate)
        {
            var day = targetDate.AddDays(to);
            var dayPage = _dayPageContents[to];
            if (!_scheduleInDays.TryGetValue(day, out var inDay)) return;
            foreach (var node in inDay)
            {
                node.SetParent(dayPage);
            }
        }

        private static bool IsCalendarModeWeek(CalendarType type)
        {
            return type is CalendarType.ThreeWeeks or CalendarType.OneMonth;
        }

        private static int ElementsInCalendar(CalendarType type)
        {
            return type switch
            {
                CalendarType.OneDay => 1,
                CalendarType.ThreeDays => 3,
                CalendarType.OneWeek => 7,
                CalendarType.ThreeWeeks => 3,
                CalendarType.OneMonth => 6,
                _ => 0
            };
        }

        private void OnDisable()
        {
            _historyService.UpdateHistory(HistoryType.PreviousCalendarType, _calendarType);
        }
    }
}