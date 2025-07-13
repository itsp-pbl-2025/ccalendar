using System;
using System.Collections.Generic;
using Presentation.Presenter;
using Presentation.Utilities;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLinq;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Presentation.Views.Extensions
{
    public class PaginateScrollRect : ScrollRect
    {
        private Action<int> _onPageSteppedCallback;
        private float _prevOffsetX, _thresholdRatio = 1.0f, _unitPageWidth = float.MaxValue;
        private int _pointersCount;
        private bool _horizontalMoved;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _pointersCount = Touch.activeTouches.Count;
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            _pointersCount = Mathf.Max(0, _pointersCount - 1);
            base.OnEndDrag(eventData);

            if (_pointersCount == 0)
            {
                var pageStepped = 0;
                while (Mathf.Abs(content.anchoredPosition.x) > _thresholdRatio * _unitPageWidth)
                {
                    var stepDirection = -Mathf.Sign(content.anchoredPosition.x);
                    if (pageStepped is not 0 && (int)Mathf.Sign(pageStepped) != (int)stepDirection) break;
                    pageStepped += (int)stepDirection;
                    content.anchoredPosition += _unitPageWidth * stepDirection * Vector2.right;
                }
                _prevOffsetX = content.anchoredPosition.x;
                _onPageSteppedCallback?.Invoke(pageStepped);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            vertical = false;
            horizontal = true;
        }
        
        private void Update()
        {
            if (!Application.isPlaying) return;
            
            if (_pointersCount is 0)
            {
                if (_horizontalMoved)
                {
                    velocity -= ReversingVelocity(content.anchoredPosition.x) * Vector2.right;
                    if ((int)Mathf.Sign(_prevOffsetX) != (int)Mathf.Sign(content.anchoredPosition.x) ||
                        Mathf.Abs(content.anchoredPosition.x) < 1f)
                    {
                        velocity = Vector2.zero;
                        content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
                        _horizontalMoved = false;
                    }
                }
                else
                {
                    content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
                }
            }
            else
            {
                _horizontalMoved = true;
            }
            _prevOffsetX = content.anchoredPosition.x;
            
            return;

            float ReversingVelocity(float offset)
            {
                return (Mathf.Sqrt(Mathf.Abs(offset) + 1000) * 1000 * Mathf.Sign(offset)) * Time.deltaTime;
            }
        }

        public void SetStepPageSettings(float unitPageWidth, float thresholdPageStep, Action<int> onPageStepped)
        {
            _unitPageWidth = unitPageWidth;
            _thresholdRatio = thresholdPageStep;
            _onPageSteppedCallback = onPageStepped;
        }
    }
}