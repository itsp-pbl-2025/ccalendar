using System;
using Presentation.Views.Common;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public abstract class PopupWindow : MonoBehaviour
    {
        [SerializeField] protected RectTransform safeRect, fixedRect;
        [SerializeField] protected RectTransform outOfWindowRect;

        public virtual void OnOpenWindow()
        {
            // play se
        }

        public virtual void SetupWithCanvas(AutoAspectCanvas canvas)
        {
            canvas.AddSafeRectTransform(safeRect);
            canvas.AddFixedRectTransform(fixedRect);
            canvas.AddFullRectTransform(outOfWindowRect);
        }

        public virtual void UnsetFromCanvas(AutoAspectCanvas canvas)
        {
            canvas.RemoveSafeRectTransform(safeRect);
            canvas.RemoveFixedRectTransform(fixedRect);
            canvas.RemoveFullRectTransform(outOfWindowRect);
        }
        
        public virtual bool EnableClosingByButton()
        {
            return true;
        }

        public virtual void CloseWindow()
        {
            if (PopupManager.Instance.ClosePopup(this))
            {
                // play se
            }
        }
    }
}