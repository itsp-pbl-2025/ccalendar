using System;
using DG.Tweening;
using Presentation.Views.Extensions;
using R3;
using TMPro;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public class TimeOnlyPopup : PopupWindow
    {
        #region CCTimeOnly
        
        public readonly struct CCTimeOnly
        {
            private readonly long _ticks;

            public int Second => (int)(_ticks / TimeSpan.TicksPerSecond) % 60;
            public int Minute => (int)(_ticks / TimeSpan.TicksPerMinute) % 60;
            public int Hour => (int)(_ticks / TimeSpan.TicksPerHour) % 24;
            
            public CCTimeOnly(long ticks)
            {
                _ticks = ticks;
            }
            
            public CCTimeOnly(int hour, int minute, int second)
            {
                _ticks = ((hour * 60 + minute) * 60 + second) * TimeSpan.TicksPerSecond;
            }

            public CCTimeOnly(DateTime dateTime)
            {
                _ticks = dateTime.Ticks - dateTime.Date.Ticks;
            }

            public CCTimeOnly AddHour(int hour)
            {
                return new CCTimeOnly(_ticks + hour * TimeSpan.TicksPerHour);
            }

            public CCTimeOnly AddMinute(int minute)
            {
                return new CCTimeOnly(_ticks + minute * TimeSpan.TicksPerMinute);
            }

            public CCTimeOnly AddSecond(int second)
            {
                return new CCTimeOnly(_ticks + second * TimeSpan.TicksPerSecond);
            }

            public DateTime GetDateTime(DateTime day)
            {
                return new DateTime(day.Date.Ticks + _ticks);
            }
        }

        #endregion

        private enum SelectState
        {
            Hour,
            Minute,
        }

        private const float TimePointerSnapping = 0.33f;
        
        [SerializeField] private ButtonWithLabel hourButton, minuteButton;
        [SerializeField] private CanvasGroup hourGroup, minuteGroup;
        [SerializeField] private RectInputHandler rectInputHandler;
        [SerializeField] private RectTransform pointerLine, pointerPoint;
        [SerializeField, Tooltip("x24")] private TextMeshProUGUI[] hourSelectors;
        [SerializeField, Tooltip("x12")] private TextMeshProUGUI[] minuteSelectors;

        private Action<CCTimeOnly> _onTimeDefined;
        private Sequence _seq;

        private CCTimeOnly _selectedTime;
        private SelectState _state;
        private int _targetHour, _targetMinute;
        private bool _isHolding;
        
        public void Init(Action<CCTimeOnly> onTimeDefined, CCTimeOnly? target = null)
        {
            _onTimeDefined = onTimeDefined;
            
            _state = SelectState.Hour;
            target ??= new CCTimeOnly(DateTime.Now);
            SetTime(target.Value);

            rectInputHandler.State.Subscribe(s =>
            {
                switch (s)
                {
                    case RectInputHandler.TouchState.Down:
                        _isHolding = true;
                        break;
                    case RectInputHandler.TouchState.Up:
                        _isHolding = false;
                        OnAreaTapUp(rectInputHandler.PositionOnRect);
                        break;
                }
            }).AddTo(this);
        }

        private void Update()
        {
            if (_isHolding) OnAreaTapping(rectInputHandler.PositionOnRect);
        }

        private void SetTime(CCTimeOnly time, bool setPoint = true)
        {
            _selectedTime = time.AddSecond(-time.Second);
            _targetHour = time.Hour;
            _targetMinute = time.Minute;
            ReloadTime(setPoint);
        }

        private void ReloadTime(bool setPoint = true)
        {
            hourGroup.alpha = _state == SelectState.Hour ? 1 : 0;
            minuteGroup.alpha = _state == SelectState.Minute ? 1 : 0;
            hourButton.Button.image.color = _state is SelectState.Hour ? Color.cyan : Color.white;
            minuteButton.Button.image.color = _state is SelectState.Minute ? Color.cyan : Color.white;
            hourButton.Label.text = _targetHour.ToString("D2");
            minuteButton.Label.text = _targetMinute.ToString("D2");

            if (setPoint)
            {
                _seq?.Kill();
                _seq = DOTween.Sequence();
                if (_state == SelectState.Hour)
                {
                    var now = pointerPoint.anchoredPosition;
                    var next = GetSuitableOffsetFromHour(_targetHour);
                    _seq.Append(DOVirtual.Vector2(now, next, TimePointerSnapping, SetPointerPosition).SetEase(Ease.OutQuad));
                }
                else
                {
                    var now = pointerPoint.anchoredPosition;
                    var next = GetSuitableOffsetFromMinute(_targetMinute);
                    _seq.Append(DOVirtual.Vector2(now, next, TimePointerSnapping, SetPointerPosition).SetEase(Ease.OutQuad));
                }
            }
        }

        public void OnPressDefaultButton()
        {
            SetTime(new CCTimeOnly(DateTime.Now));
        }

        public void OnPressStateButton(bool setMinute)
        {
            _state = setMinute ? SelectState.Minute : SelectState.Hour;
            ReloadTime();
        }

        public void OnPressDefineButton()
        {
            _onTimeDefined?.Invoke(_selectedTime);
            CloseWindow();
        }

        private void OnAreaTapDown(Vector2 pos)
        {
            _seq?.Kill(true);
        }

        private void OnAreaTapping(Vector2 pos)
        {
            SetPointerPosition(GetSuitableOffsetOfPointer(pos));

            if (_state == SelectState.Hour)
            {
                var hourIndex = GetSuitableHourFromOffset(pos);
                _targetHour = hourIndex;
            }
            else
            {
                var minute = GetMinuteFromOffset(pos);
                _targetMinute = minute;
            }

            ReloadTime(false);
        }

        private void OnAreaTapUp(Vector2 pos)
        {
            if (_state == SelectState.Hour)
            {
                var hourIndex = GetSuitableHourFromOffset(pos);
                _state = SelectState.Minute;
                SetTime(new CCTimeOnly(hourIndex, _targetMinute, 0));
            }
            else
            {
                var minute = GetMinuteFromOffset(pos);
                SetTime(new CCTimeOnly(_targetHour, minute, 0));
            }
        }

        private int GetSuitableHourFromOffset(Vector2 off)
        {
            var index = 0;
            var distance = float.MaxValue;
            
            for (var i = 0; i < hourSelectors.Length; i++)
            {
                var hour = hourSelectors[i];
                var diff = Vector2.Distance(off, hour.rectTransform.anchoredPosition);
                if (distance > diff)
                {
                    distance = diff;
                    index = i;
                }
            }

            return index;
        }

        private int GetMinuteFromOffset(Vector2 off)
        {
            if (off == Vector2.zero) return 0;

            var angleDeg = Mathf.Atan2(off.y, off.x) * Mathf.Rad2Deg;

            if (angleDeg < 0) angleDeg += 360;

            var adjustedAngle = 90 - angleDeg;
            if (adjustedAngle < 0) adjustedAngle += 360;

            return Mathf.RoundToInt(adjustedAngle / 6f) % 60;
        }

        private Vector2 GetSuitableOffsetOfPointer(Vector2 pos)
        {
            RectTransform nearest = null;
            var distance = float.MaxValue;
            if (_state is SelectState.Hour)
            {
                foreach (var hour in hourSelectors)
                {
                    var diff = Vector2.Distance(pos, hour.rectTransform.anchoredPosition);
                    if (distance > diff)
                    {
                        distance = diff;
                        nearest = hour.rectTransform;
                    }
                }
            }
            else
            {
                foreach (var minute in minuteSelectors)
                {
                    var diff = Vector2.Distance(pos, minute.rectTransform.anchoredPosition);
                    if (distance > diff)
                    {
                        distance = diff;
                        nearest = minute.rectTransform;
                    }
                }
            }

            if (nearest is null) return pos;
            return pos.normalized * nearest.anchoredPosition.magnitude;
        }

        private Vector2 GetSuitableOffsetFromHour(int hour)
        {
            return hourSelectors[hour].rectTransform.anchoredPosition;
        }

        private Vector2 GetSuitableOffsetFromMinute(int minute)
        {
            var angle = (90f - (minute % 60) * 6f) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * minuteSelectors[0].rectTransform.anchoredPosition.magnitude;
        }

        private void SetPointerPosition(Vector2 pos)
        {
            pointerPoint.anchoredPosition = pos;
            pointerLine.sizeDelta = new Vector2(pos.magnitude, pointerLine.sizeDelta.y);
            pointerLine.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
        }
    }
}