using DG.Tweening;
using Domain.Entity;
using Presentation.Presenter;
using Presentation.Views.Extensions;
using Presentation.Views.Popup;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class PartsScheduleInDay : MonoBehaviour
    {
        private const string ScheduleTitleDefault = "(タイトルなし)";
        private const float TimeTransform = 0.25f;
        private const float TimeDecay = 0.125f;
        private const float PpuMultiplierInDay = 32f;
        private const float PpuMultiplierInWeek = 48f;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private ButtonRP button;
        [SerializeField] private ImageRx imageFrame;
        [SerializeField] private LabelRx scheduleLabel;

        private UnitSchedule _schedule;
        private CCDateOnly _inDate;

        private Sequence _seq;
        private bool _mortal;
        
        public bool Init(UnitSchedule schedule, CCDateOnly date)
        {
            if (_mortal) return false;
            if (!schedule.Duration.IsCollided(new ScheduleDuration(date))) return false;
            
            _schedule = schedule;
            _inDate = date;
            ReloadLabel();
            
            return true;
        }

        public void TransformInDay(float left, float right, bool instant = false)
        {
            const float fontSizeMax = 36f;
            const float fontSizeMin = 32f;
            
            if (_mortal) return;
            
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            var bottom = (CCTimeOnly.SecondsInDay - _schedule.Duration.EndTime.ToTimeOnly().GetAllSeconds()) / (float)CCTimeOnly.SecondsInDay;
            var top = (CCTimeOnly.SecondsInDay - _schedule.Duration.StartTime.ToTimeOnly().GetAllSeconds()) / (float)CCTimeOnly.SecondsInDay;

            var anchorMinTo = new Vector2(left, bottom);
            var anchorMaxTo = new Vector2(right, top);

            imageFrame.pixelsPerUnitMultiplier = PpuMultiplierInDay;
            if (instant)
            {
                rectTransform.anchorMin = anchorMinTo;
                rectTransform.anchorMax = anchorMaxTo;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
                scheduleLabel.fontSizeMax = fontSizeMax;
                scheduleLabel.fontSizeMin = fontSizeMin;
            }
            else
            {
                var anchorMinFrom = rectTransform.anchorMin;
                var anchorMaxFrom = rectTransform.anchorMax;
                var positionFrom = rectTransform.anchoredPosition;
                var sizeFrom = rectTransform.sizeDelta;
                var labelFontMaxFrom = scheduleLabel.fontSizeMax;
                var labelFontMinFrom = scheduleLabel.fontSizeMin;
                
                _seq.Append(DOVirtual.Float(0, 1, TimeTransform, v =>
                {
                    rectTransform.anchorMin = Vector2.Lerp(anchorMinFrom, anchorMinTo, v);
                    rectTransform.anchorMax = Vector2.Lerp(anchorMaxFrom, anchorMaxTo, v);
                    rectTransform.anchoredPosition = Vector2.Lerp(positionFrom, Vector2.zero, v);
                    rectTransform.sizeDelta = Vector2.Lerp(sizeFrom, Vector2.zero, v);
                    scheduleLabel.fontSizeMin = Mathf.Lerp(labelFontMinFrom, fontSizeMin, v);
                    scheduleLabel.fontSizeMax = Mathf.Lerp(labelFontMaxFrom, fontSizeMax, v);
                }));
            }
        }

        public void TransformInWeek(float offsetY, float height, bool instant = false)
        {
            const float fontSizeMax = 20f;
            const float fontSizeMin = 18f;

            if (_mortal) return;
            
            _seq?.Complete();
            _seq = DOTween.Sequence();
            
            var positionTo = new Vector2(0, -offsetY);
            var sizeTo = new Vector2(0, height);
            
            imageFrame.pixelsPerUnitMultiplier = PpuMultiplierInWeek;
            if (instant)
            {
                rectTransform.anchorMin = Vector2.up;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.anchoredPosition = positionTo;
                rectTransform.sizeDelta = sizeTo;
                scheduleLabel.fontSizeMax = fontSizeMax;
                scheduleLabel.fontSizeMin = fontSizeMin;
            }
            else
            {
                var anchorMinFrom = rectTransform.anchorMin;
                var anchorMaxFrom = rectTransform.anchorMax;
                var positionFrom = rectTransform.anchoredPosition;
                var sizeFrom = rectTransform.sizeDelta;
                var labelFontMaxFrom = scheduleLabel.fontSizeMax;
                var labelFontMinFrom = scheduleLabel.fontSizeMin;
                
                _seq.Append(DOVirtual.Float(0, 1, TimeTransform, v =>
                {
                    rectTransform.anchorMin = Vector2.Lerp(anchorMinFrom, Vector2.up, v);
                    rectTransform.anchorMax = Vector2.Lerp(anchorMaxFrom, Vector2.one, v);
                    rectTransform.anchoredPosition = Vector2.Lerp(positionFrom, positionTo, v);
                    rectTransform.sizeDelta = Vector2.Lerp(sizeFrom, sizeTo, v);
                    scheduleLabel.fontSizeMin = Mathf.Lerp(labelFontMinFrom, fontSizeMin, v);
                    scheduleLabel.fontSizeMax = Mathf.Lerp(labelFontMaxFrom, fontSizeMax, v);
                }));
            }
        }

        public void OnPress()
        {
            if (PopupManager.Instance.ShowPopupUnique(InAppContext.Prefabs.GetPopup<ScheduleCreationPopup>(),
                    out var window))
            {
                window.Init(_schedule);
            }
        }

        public void SetParent(RectTransform parent, bool keepPosition = false)
        {
            if (keepPosition)
            {
                var prevPosition = transform.position;
                rectTransform.SetParent(parent);
                transform.position = prevPosition;
            }
            else
            {
                rectTransform.SetParent(parent);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        public void Decay(bool instant = false)
        {
            if (instant)
            {
                Destroy(gameObject);
                return;
            }
            
            _mortal = true;
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            _seq.Append(imageFrame.DOFade(0f, TimeDecay))
                .Join(scheduleLabel.DOFade(0f, TimeDecay))
                .OnComplete(() => Destroy(gameObject));
        }

        private void ReloadLabel()
        {
            var title = string.IsNullOrWhiteSpace(_schedule.Title) ? ScheduleTitleDefault : _schedule.Title;
            var description = string.IsNullOrWhiteSpace(_schedule.Description) ? "" : '\n'+_schedule.Description;
            scheduleLabel.text = $"{title}{description}";
        }
    }
}