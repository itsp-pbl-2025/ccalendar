using DG.Tweening;
using Domain.Entity;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class PartsScheduleInDay : MonoBehaviour
    {
        private const string ScheduleTitleDefault = "(タイトルなし)";
        private const float TimeTransform = 0.25f;
        private const float TimeDecay = 0.125f;

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

        public void Transform(float left, float right, bool instant = false)
        {
            if (_mortal)  return;
            
            _seq?.Complete();
            _seq = DOTween.Sequence();
            
            var bottom = (CCTimeOnly.SecondsInDay - _schedule.Duration.EndTime.ToTimeOnly().GetAllSeconds()) / (float)CCTimeOnly.SecondsInDay;
            var top = (CCTimeOnly.SecondsInDay - _schedule.Duration.StartTime.ToTimeOnly().GetAllSeconds()) / (float)CCTimeOnly.SecondsInDay;

            var bottomLeft = new Vector2(left, bottom);
            var topRight = new Vector2(right, top);
            
            if (instant)
            {
                rectTransform.anchorMin = bottomLeft;
                rectTransform.anchorMax = topRight;
            }
            else
            {
                _seq.Append(DOVirtual.Vector2(rectTransform.anchorMin, bottomLeft, TimeTransform, v =>
                {
                    rectTransform.anchorMin = v;
                })).Join(DOVirtual.Vector2(rectTransform.anchorMax, topRight, TimeTransform, v =>
                {
                    rectTransform.anchorMax = v;
                }));
            }
        }

        public void SetParent(RectTransform parent, bool keepPosition = true)
        {
            rectTransform.SetParent(parent);
            if (keepPosition) rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Decay()
        {
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