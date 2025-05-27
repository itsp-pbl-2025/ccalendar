using TMPro;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public class RectFigurePopup : PopupWindow
    {
        // right, top, left, bottom
        private static readonly Vector4 Margin = new Vector4(40f, 40f, 40f, 40f);
        
        [SerializeField] private RectTransform figureParent, windowRect;
        
        public void Init(RectTransform rectTransform)
        {
            var size = rectTransform.sizeDelta;
            // ウィンドウのサイズを設定
            windowRect.sizeDelta = new Vector2(Margin.x + Margin.z, Margin.y + Margin.w) + size;
            var figure = Instantiate(rectTransform, figureParent);
        }
    }
}