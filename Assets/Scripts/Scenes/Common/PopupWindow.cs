using UnityEngine;

namespace Scenes.Common
{
    public abstract class PopupWindow : MonoBehaviour
    {
        [SerializeField] private RectTransform safeRect, fixedRect;
        [SerializeField] private RectTransform outOfWindowRect;

        public virtual void OnOpenWindow()
        {
            // play se
        }

        public void SetupWithCanvas(AutoAspectCanvas canvas)
        {
            canvas.AddSafeRectTransform(safeRect);
            canvas.AddFixedRectTransform(fixedRect);
            canvas.AddFullRectTransform(outOfWindowRect);
        }

        public void UnsetFromCanvas(AutoAspectCanvas canvas)
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