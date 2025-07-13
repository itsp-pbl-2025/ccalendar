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
        [SerializeField] private ButtonWithLabel yearButtonInWeek, monthButtonInWeek;
        [SerializeField] private ScalableScrollRect dayScrollRect;
        [SerializeField] private PaginateScrollRect weekScrollRect;
        [SerializeField] private RectTransform dayContainer, weekContainer, animationContainer;
        [SerializeField] private RectTransform dayPageContent;
        [SerializeField] private PartsMonthlyContainer weekPageContent;
        [SerializeField] private PartsDayIconLabel dayFullIconPrefab, dayMiniIconPrefab;
        [SerializeField] private RectTransform dateIconContent;
        [SerializeField] private ImageRx daySeparatorPrefab;
        
        private ScheduleService _scheduleService;
        private HistoryService _historyService;
        
        private CalendarType _calendarType = CalendarType.Invalid;
        public CCDateOnly CurrentTargetDate => _targetDate;
        private CCDateOnly _targetDate;
        private readonly Dictionary<CCDateOnly, List<UnitSchedule>> _cachedSchedules = new();
        
        private readonly Dictionary<int, PartsDayIconLabel> _dayIcons = new();
        private List<PartsWeekDaysLabel> _weekSeparators;
        
        private readonly Dictionary<int, RectTransform> _dayPageContents = new();
        private readonly Dictionary<int, PartsMonthlyContainer> _weekPageContents = new();
        
        private readonly Dictionary<int, List<PartsScheduleInDay>> _scheduleInDays = new();
        private readonly Dictionary<int, Dictionary<int, List<PartsScheduleInDay>>> _scheduleInMonths = new();
        
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
            var initAlpha = 1f;
            
            _typeSeq?.Complete();
            _typeSeq = DOTween.Sequence().AppendInterval(TimeSwitchType);
            
            // 初期状態なら、全部一回透明にしちゃう
            if (fromInvalid)
            {
                initAlpha = 0f;
                dayCanvas.alpha = 0f;
                dayCanvas.interactable = false;
                dayCanvas.blocksRaycasts = false;
                weekCanvas.alpha = 0f;
                weekCanvas.interactable = false;
                weekCanvas.blocksRaycasts = false;
            }

            var targetMoveOffset = 0;
            // どのくらいスクロールしたらページを隣にするか？
            switch (type)
            {
                case CalendarType.OneDay:
                    dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, StepScrollPage);
                    break;
                case CalendarType.ThreeDays:
                    dayScrollRect.SetStepPageSettings(WidthDayContainer/3, ThresholdPageSwitch, StepScrollPage);
                    break;
                case CalendarType.OneWeek:
                    targetMoveOffset = -(int)_targetDate.ToDateTime().DayOfWeek;
                    _targetDate = _targetDate.AddDays(targetMoveOffset);
                    
                    dayScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, StepScrollPage);
                    break;
                case CalendarType.OneMonth:
                    _targetDate = _targetDate.AddDays(1 - _targetDate.ToDateTime().Day);
                    
                    weekScrollRect.SetStepPageSettings(WidthDayContainer, ThresholdPageSwitch, StepScrollPage);
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
                    dayCanvas.blocksRaycasts = false;
                    _modeSeq.Append(DOVirtual.Float(initAlpha, 0f, TimeSwitchMode, a => dayCanvas.alpha = a))
                        .Join(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => weekCanvas.alpha = a))
                        .OnComplete(() =>
                        {
                            weekCanvas.interactable = true;
                            weekCanvas.blocksRaycasts = true;
                        });

                    if (!fromInvalid)
                    {
                        RemoveAllDayContainers();
                    }
                }
                else
                {
                    weekCanvas.interactable = false;
                    weekCanvas.blocksRaycasts = false;
                    _modeSeq.Append(DOVirtual.Float(initAlpha, 0f, TimeSwitchMode, a => weekCanvas.alpha = a))
                        .Join(DOVirtual.Float(0f, 1f, TimeSwitchMode, a => dayCanvas.alpha = a))
                        .OnComplete(() =>
                        {
                            dayCanvas.interactable = true;
                            dayCanvas.blocksRaycasts = true;
                        });

                    if (!fromInvalid)
                    {
                        RemoveAllMonthlyContainers();
                    }
                }

                InitPageAll(type);
            }
            else if (nextModeIsWeek)
            {
                // WeekModeはOneMonthしかないので切り替え所作が発生するはずはない
                throw new ArgumentOutOfRangeException(nameof(_calendarType), "CalendarType.OneMonth is only type belongs to WeekMode.");
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
                                var unitAnchor = 1f / (3 * ElementsInCalendar(_calendarType));
                                _modeSeq
                                    .Join(DOVirtual.Float(1 / 3f + unitAnchor * offset, 1 / 3f * (offset + 1),
                                        TimeSwitchMode, v => dayPage.anchorMin = new Vector2(v, 0)))
                                    .Join(DOVirtual.Float(1 / 3f + unitAnchor * (offset + 1), 1 / 3f * (offset + 2),
                                        TimeSwitchMode, v => dayPage.anchorMax = new Vector2(v, 1)));
                                
                                RefreshTypedDayIcon(offset, _targetDate, type);
                                FillScheduleInDayContainer(offset, _targetDate);
                            }
                            else
                            {
                                RemoveDayContainer(offset);
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
                                    RefreshTypedDayIcon(offset, _targetDate, CalendarType.ThreeDays);
                                }
                                else
                                {
                                    RemoveDayContainer(offset);
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

                        if (targetMoveOffset == 0) break;
                        if (_calendarType is CalendarType.OneDay)
                        {
                            if (Mathf.Sign(targetMoveOffset) > 0)
                            {
                                for (var offset = -1; offset <= 1; offset++)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-targetMoveOffset);
                                }
                            }
                            else
                            {
                                for (var offset = 1; offset >= -1; offset--)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-targetMoveOffset);
                                }
                            }
                        }
                        else if (_calendarType is CalendarType.ThreeDays)
                        {
                            if (Mathf.Sign(targetMoveOffset) > 0)
                            {
                                for (var offset = -3; offset <= 5; offset++)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-targetMoveOffset);
                                }
                            }
                            else
                            {
                                for (var offset = 5; offset >= -3; offset--)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-targetMoveOffset);
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(_calendarType), _calendarType, null);
                        }
                        break;
                    case CalendarType.OneMonth: default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }

            _calendarType = type;
        }

        private void InitPageAll(CalendarType type)
        {
            var containerInPage = ElementsInCalendar(type);
            switch (type)
            {
                case CalendarType.OneDay:
                case CalendarType.ThreeDays:
                case CalendarType.OneWeek:
                    for (var offset = -containerInPage; offset < 2 * containerInPage; offset++)
                    {
                        CreateSingleDayContainer(offset, _targetDate, type);
                    }
                    break;
                case CalendarType.OneMonth:
                    for (var offset = -containerInPage; offset < 2 * containerInPage; offset++)
                    {
                        CreateSingleMonthlyContainer(offset, _targetDate.AddMonths(offset));
                    }
                    break;
            }
        }

        private void StepScrollPage(int stepPages)
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
                            CleanupAllDaySchedules(true);
                            
                            for (var offset = -1; offset < 2; offset++)
                            {
                                FillScheduleInDayContainer(offset, _targetDate);
                            }
                            break;
                        case > 0:
                            for (var offset = -1; offset < 2+stepPages; offset++)
                            {
                                if (offset < -1+stepPages)
                                {
                                    CleanupDayContainer(offset, true);
                                }
                                else if (offset < 2)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-stepPages);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset-stepPages, _targetDate);
                                }
                            }
                            break;
                        case < 0:
                            for (var offset = 1; offset > -2+stepPages; offset--)
                            {
                                if (offset > 1+stepPages)
                                {
                                    CleanupDayContainer(offset, true);
                                }
                                else if (offset > -2)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-stepPages);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset-stepPages, _targetDate);
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
                            CleanupAllDaySchedules(true);
                            
                            for (var offset = -3; offset < 6; offset++)
                            {
                                FillScheduleInDayContainer(offset, _targetDate);
                            }
                            break;
                        case > 0:
                            for (var offset = -3; offset < 6+stepPages; offset++)
                            {
                                if (offset < -3+stepPages)
                                {
                                    CleanupDayContainer(offset, true);
                                }
                                else if (offset <= 6)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-stepPages);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset-stepPages, _targetDate);
                                }
                            }
                            break;
                        case < 0:
                            for (var offset = 5; offset > -4+stepPages; offset--)
                            {
                                if (offset > 5+stepPages)
                                {
                                    CleanupDayContainer(offset, true);
                                }
                                else if (offset > -4)
                                {
                                    MoveAllScheduleToDayContainer(offset, offset-stepPages);
                                }
                                else
                                {
                                    FillScheduleInDayContainer(offset-stepPages, _targetDate);
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
                            CleanupAllDaySchedules(true);
                            
                            for (var offset = -7; offset < 14; offset++)
                            {
                                FillScheduleInDayContainer(offset, _targetDate);
                            }
                            break;
                        case > 0:
                            for (var offset = -1; offset < 2+stepPages; offset++)
                            {
                                if (offset < -1+stepPages)
                                {
                                    for (var j=0; j<7; j++) CleanupDayContainer(offset*7+j, true);
                                }
                                else if (offset < 2)
                                {
                                    for (var j=0; j<7; j++) MoveAllScheduleToDayContainer(offset*7+j, (offset-stepPages)*7+j);
                                }
                                else
                                {
                                    for (var j=0; j<7; j++) FillScheduleInDayContainer((offset-stepPages)*7+j, _targetDate);
                                }
                            }
                            break;
                        case < 0:
                            for (var offset = 1; offset > -2+stepPages; offset--)
                            {
                                if (offset > 1+stepPages)
                                {
                                    for (var j=6; j>=0; j--) CleanupDayContainer(offset*7+j, true);
                                }
                                else if (offset >= -2)
                                {
                                    for (var j=6; j>=0; j--) MoveAllScheduleToDayContainer(offset*7+j, (offset-stepPages)*7+j);
                                }
                                else
                                {
                                    for (var j=6; j>=0; j--) FillScheduleInDayContainer((offset-stepPages)*7+j, _targetDate);
                                }
                            }
                            break;
                    }

                    break;
                }
                case CalendarType.OneMonth:
                {
                    _targetDate = _targetDate.AddMonths(stepPages);

                    switch (stepPages)
                    {
                        case > 2 or < -2:
                            CleanupAllMonthlySchedules(true);
                            
                            for (var offset = -1; offset < 2; offset++)
                            {
                                FillScheduleInDayContainer(offset, _targetDate.AddMonths(offset));
                            }
                            break;
                        case > 0:
                            for (var offset = -1; offset < 2+stepPages; offset++)
                            {
                                if (offset < -1+stepPages)
                                {
                                    CleanupMonthlyContainer(offset, true);
                                }
                                else if (offset < 2)
                                {
                                    MoveAllScheduleToMonthlyContainer(offset, offset-stepPages, _targetDate.AddMonths(offset-stepPages));
                                }
                                else
                                {
                                    FillScheduleInMonthlyContainer(offset-stepPages, _targetDate.AddMonths(offset-stepPages));
                                }
                            }
                            break;
                        case < 0:
                            for (var offset = 1; offset > -2+stepPages; offset--)
                            {
                                if (offset > 1+stepPages)
                                {
                                    CleanupMonthlyContainer(offset, true);
                                }
                                else if (offset > -2)
                                {
                                    MoveAllScheduleToMonthlyContainer(offset, offset-stepPages, _targetDate.AddMonths(offset-stepPages));
                                }
                                else
                                {
                                    FillScheduleInMonthlyContainer(offset-stepPages, _targetDate.AddMonths(offset-stepPages));
                                }
                            }
                            break;
                    }
                    break;
                }
            }
        }

        #region DayContainers

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
            RefreshTypedDayIcon(offset, targetDate, calendarType);
            
            var containersInPage = ElementsInCalendar(calendarType);
            var widthInAnchor = 1f / (3 * containersInPage);
            var dayPage = Instantiate(dayPageContent, dayContainer);
            dayPage.anchorMin = new Vector2(widthInAnchor * (offset + containersInPage), 0f);
            dayPage.anchorMax = new Vector2(widthInAnchor * (offset + containersInPage + 1), 1f);
            _dayPageContents.Add(offset, dayPage);

            FillScheduleInDayContainer(offset, targetDate, dayPage);
        }

        /// <summary>
        /// 特定の日付におけるスケジュールを再読み込みし、スケジュールオブジェクトを作成する。
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        /// <param name="targetDate">更新の基準となる日付</param>
        private void FillScheduleInDayContainer(int offset, CCDateOnly targetDate, RectTransform dayPage = null)
        {
            if (dayPage is null && !_dayPageContents.TryGetValue(offset, out dayPage)) return;
            
            var day = targetDate.AddDays(offset);
            
            CleanupDayContainer(offset);
            
            var inDay = new List<PartsScheduleInDay>();
            foreach (var schedule in GetSchedulesInDay(day))
            {
                var node = Instantiate(schedulePrefab, dayPage);
                node.Init(schedule, day);
                node.TransformInDay(0f, 1f);
                inDay.Add(node);
            }
            _scheduleInDays.Add(offset, inDay);
        }

        /// <summary>
        /// 特定の日付におけるスケジュールオブジェクトを削除する
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        /// <param name="instant"></param>
        private void CleanupDayContainer(int offset, bool instant = false)
        {
            if (_scheduleInDays.Remove(offset, out var inDay))
            {
                foreach (var node in inDay)
                {
                    // node.SetParent(animationContainer);
                    node.Decay(instant);
                }
            }
        }

        private void CleanupAllDaySchedules(bool instant = false)
        {
            foreach (var (_, inDay) in _scheduleInDays)
            {
                foreach (var node in inDay)
                {
                    node.Decay(instant);
                }
            }
            _scheduleInDays.Clear();
        }

        /// <summary>
        /// 特定の日付コンテナを削除する
        /// </summary>
        /// <param name="offset">日付からの日距離</param>
        private void RemoveDayContainer(int offset)
        {
            CleanupDayContainer(offset);
            if (_dayIcons.Remove(offset, out var dayIcon)) Destroy(dayIcon.gameObject);
            if (_dayPageContents.Remove(offset, out var dayPage)) Destroy(dayPage.gameObject);
        }

        /// <summary>
        /// ページ移動に伴ってズレたスケジュールオブジェクトを正しい親に帰属させる
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void MoveAllScheduleToDayContainer(int from, int to)
        {
            if (!_dayPageContents.TryGetValue(to, out var dayPage)) return;
            if (!_scheduleInDays.Remove(from, out var inDay)) return;
            CleanupDayContainer(to);
            _scheduleInDays.Add(to, inDay);
            foreach (var node in inDay)
            {
                node.SetParent(dayPage);
            }
        }

        private void RemoveAllDayContainers()
        {
            foreach (var (_, inDay) in _scheduleInDays)
            {
                foreach (var node in inDay)
                {
                    // node.SetParent(animationContainer);
                    node.Decay();
                }
                inDay.Clear();
            }
            _scheduleInDays.Clear();

            foreach (var (_, dayIcon) in _dayIcons)
            {
                Destroy(dayIcon.gameObject);
            }
            _dayIcons.Clear();

            foreach (var (_, dayPage) in _dayPageContents)
            {
                Destroy(dayPage.gameObject);
            }
            _dayPageContents.Clear();
        }
        
        #endregion

        #region MonthlyContainers

        /// <summary>
        /// 月別コンテナを作成
        /// </summary>
        /// <param name="offset">ページが位置関係的にどこにあるか</param>
        /// <param name="initMonth">月の最初の日</param>
        private void CreateSingleMonthlyContainer(int offset, CCDateOnly initMonth)
        {
            var weekPage = Instantiate(weekPageContent, weekContainer);
            _weekPageContents.Add(offset, weekPage);
            
            FillScheduleInMonthlyContainer(offset, initMonth, weekPage);
        }
        
        private void FillScheduleInMonthlyContainer(int offset, CCDateOnly initMonth, PartsMonthlyContainer weekPage = null)
        {
            if (weekPage is null && !_weekPageContents.TryGetValue(offset, out weekPage)) return;

            if (offset is 0)
            {
                yearButtonInWeek.Label.text = $"{initMonth.Year.Value}年";
                monthButtonInWeek.Label.text = $"{initMonth.Month.Value}月";
            }
            
            var initDate = initMonth.AddDays(-(int)initMonth.ToDateTime().DayOfWeek);
            weekPage.Init(initDate);
            weekPage.SetOffset(offset);
            
            CleanupMonthlyContainer(offset);
            var inMonth = new Dictionary<int, List<PartsScheduleInDay>>();
            var (startDate, endDate) = weekPage.GetScheduleRange();
            var index = 0;
            for (var date = startDate; date.CompareTo(endDate) <= 0; date = startDate.AddDays(++index))
            {
                var inDay = new List<PartsScheduleInDay>();
                foreach (var schedule in GetSchedulesBeginInDay(date))
                {
                    var node = Instantiate(schedulePrefab, animationContainer);
                    node.Init(schedule, date);
                    inDay.Add(node);
                }
                weekPage.PlaceSchedules(index, inDay, true);
                inMonth.Add(index, inDay);
            }
            _scheduleInMonths.Add(offset, inMonth);
        }

        private void CleanupMonthlyContainer(int offset, bool instant = false)
        {
            if (_scheduleInMonths.Remove(offset, out var inDay))
            {
                foreach (var (_, schedules) in inDay)
                {
                    foreach (var node in schedules)
                    {
                        node.SetParent(animationContainer);
                        node.Decay(instant);
                    }
                }
            }
        }

        private void CleanupAllMonthlySchedules(bool instant = false)
        {
            foreach (var (_, inMonth) in _scheduleInMonths)
            {
                foreach (var (_, inDay) in inMonth)
                {
                    foreach (var node in inDay)
                    {
                        node.SetParent(animationContainer);
                        node.Decay(instant);
                    }
                }
            }
            _scheduleInMonths.Clear();
        }

        private void RemoveMonthlyContainer(int offset)
        {
            CleanupDayContainer(offset);
            if (_weekPageContents.Remove(offset, out var dayPage)) Destroy(dayPage.gameObject);
        }

        /// <summary>
        /// ページ移動に伴ってズレたスケジュールオブジェクトを正しい親に帰属させる
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="initMonth"></param>
        private void MoveAllScheduleToMonthlyContainer(int from, int to, CCDateOnly initMonth)
        {
            if (!_weekPageContents.TryGetValue(to, out var weekPage)) return;
            if (!_scheduleInMonths.Remove(from, out var inMonth)) return;
            CleanupMonthlyContainer(to);
            
            if (to is 0)
            {
                yearButtonInWeek.Label.text = $"{initMonth.Year.Value}年";
                monthButtonInWeek.Label.text = $"{initMonth.Month.Value}月";
            }
            
            weekPage.Init(initMonth.AddDays(-(int)initMonth.ToDateTime().DayOfWeek));
            _scheduleInMonths.Add(to, inMonth);
            foreach (var (index, inDay) in inMonth)
            {
                weekPage.PlaceSchedules(index, inDay, true);
            }
        }

        private void RemoveAllMonthlyContainers()
        {
            foreach (var (_, inMonth) in _scheduleInMonths)
            {
                foreach (var (_, inDay) in inMonth)
                {
                    foreach (var node in inDay)
                    {
                        node.SetParent(animationContainer);
                        node.Decay();
                    }
                }
            }
            _scheduleInDays.Clear();

            foreach (var (_, weekPage) in _weekPageContents)
            {
                Destroy(weekPage.gameObject);
            }
            _weekPageContents.Clear();
        }

        #endregion
        
        private static bool IsCalendarModeWeek(CalendarType type)
        {
            return type is CalendarType.OneMonth;
        }

        private static int ElementsInCalendar(CalendarType type)
        {
            return type switch
            {
                CalendarType.OneDay => 1,
                CalendarType.ThreeDays => 3,
                CalendarType.OneWeek => 7,
                CalendarType.OneMonth => 1,
                _ => 0
            };
        }

        private List<UnitSchedule> GetSchedulesInDay(CCDateOnly date)
        {
            if (_cachedSchedules.TryGetValue(date, out var schedules))
            {
                return schedules;
            }

            var data = _scheduleService.GetSchedulesInDuration(new ScheduleDuration(date));
            _cachedSchedules.Add(date, data);
            
            return data;
        }

        private List<UnitSchedule> GetSchedulesBeginInDay(CCDateOnly date)
        {
            var ret = new List<UnitSchedule>();
            if (_cachedSchedules.TryGetValue(date, out var schedules))
            {
                foreach (var schedule in schedules)
                {
                    if (schedule.Duration.StartTime.ToDateOnly().CompareTo(date) != 0)
                    {
                        continue;
                    }
                    ret.Add(schedule);
                }
                return ret;
            }

            var data = _scheduleService.GetSchedulesInDuration(new ScheduleDuration(date));
            _cachedSchedules.Add(date, data);
            
            foreach (var schedule in data)
            {
                if (schedule.Duration.StartTime.ToDateOnly().CompareTo(date) != 0)
                {
                    continue;
                }
                ret.Add(schedule);
            }
            return ret;
        }

        private void OnDisable()
        {
            _historyService.UpdateHistory(HistoryType.PreviousCalendarType, _calendarType);
        }
    }
}