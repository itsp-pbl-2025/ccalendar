using Cysharp.Threading.Tasks;
using DG.Tweening;
using Presentation.Views.Common;
using Presentation.Views.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Sample
{
    public class SampleSidebarPopup : PopupWindow
    {
        [SerializeField] private Image windowRect;
        [SerializeField] private RectTransform contentRect;
        [SerializeField] private Button[] menuButtons;
        
        private AutoPaginator _autoPaginator;
        private Sequence _seq;

        private bool _isClosing;
        
        public void Init(AutoPaginator autoPaginator)
        {
            _autoPaginator = autoPaginator;
        }

        public void SetPage(int page)
        {
            _autoPaginator.ScrollPageTo(page);
        }

        public void ApplicationQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public override void SetupWithCanvas(AutoAspectCanvas canvas)
        {
            base.SetupWithCanvas(canvas);
            
            // WindowはFullRectに、ContentはSafeRectに準拠するので、WindowをContentに合わせて少し伸ばす
            var topOffset = canvas.CanvasRect.sizeDelta.y * (1f - safeRect.anchorMax.y);
            windowRect.rectTransform.sizeDelta += Vector2.up * topOffset;
        }

        public override void OnOpenWindow()
        {
            const float timeSlideIn = 0.2f;
            base.OnOpenWindow();

            // サイドバーなので、最初にスライドインする
            var windowInitPosition = windowRect.rectTransform.anchoredPosition;
            var contentInitPosition = contentRect.anchoredPosition;
            var moveOffset = windowRect.rectTransform.sizeDelta.x;
            
            // 初期位置をずらして、あたかも今からスライドしてくるかのようにする
            windowRect.rectTransform.anchoredPosition += Vector2.right * moveOffset;;
            contentRect.anchoredPosition += Vector2.right * moveOffset;
            
            _seq?.Kill();
            _seq = DOTween.Sequence();
            _seq.Append(windowRect.rectTransform.DOAnchorPos(windowInitPosition, timeSlideIn).SetEase(Ease.OutQuad))
                .Join(contentRect.DOAnchorPos(contentInitPosition, timeSlideIn).SetEase(Ease.OutQuad));
        }
        
        public override void CloseWindow()
        {
            if (_isClosing) return;
            
            // 閉じる命令が出てすぐ消えるのは不自然なので非同期に投げる
            CloseWindowAsync().Forget();
        }

        private async UniTask CloseWindowAsync()
        {
            const float timeSlideOut = 0.2f;

            // 動作中にもう一回動かれたら面倒なので全ての入力をシャットアウト
            _isClosing = true;
            foreach (var button in menuButtons)
            {
                button.interactable = false;
            }
            
            // 画面端に動く
            var moveOffset = windowRect.rectTransform.sizeDelta.x;
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            _seq.Append(windowRect.rectTransform.DOAnchorPosX(moveOffset, timeSlideOut).SetRelative(true).SetEase(Ease.InQuad))
                .Join(contentRect.DOAnchorPosX(moveOffset, timeSlideOut).SetRelative(true).SetEase(Ease.InQuad));
            
            // 動き終わるまで待機して、終わったらウィンドウを削除
            await _seq.AsyncWaitForCompletion();
            base.CloseWindow();
        }
    }
}