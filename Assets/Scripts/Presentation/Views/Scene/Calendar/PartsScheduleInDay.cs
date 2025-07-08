using DG.Tweening;
using Domain.Entity;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class PartsScheduleInDay : MonoBehaviour
    {
        private const float TimeTransform = 0.25f;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private ButtonRP button;
        [SerializeField] private ImageRx imageFrame;
        [SerializeField] private LabelRx scheduleLabel;

        private Schedule _schedule;
        private CCDateOnly _inDate;

        private Sequence _seq;
        
        public bool Init(Schedule schedule, CCDateOnly date)
        {
            if (!schedule.Duration.IsCollided(new ScheduleDuration(date))) return false;
            
            _schedule = schedule;
            _inDate = date;
            return true;
        }

        public void Transform(Vector2 bottomLeft, Vector2 topRight, bool instant = false)
        {
            _seq?.Complete();
            _seq = DOTween.Sequence();

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

        public void SetParent(RectTransform parent)
        {
            rectTransform.SetParent(parent);
        }
    }
}