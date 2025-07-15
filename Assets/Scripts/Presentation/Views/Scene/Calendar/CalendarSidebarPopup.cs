using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Presentation.Presenter;
using Presentation.Views.Extensions;
using Presentation.Views.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Scene.Calendar
{
    public class CalendarSidebarPopup : PopupWindow
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        
        private CalendarManager _calendarManager;
        private Sequence _seq;
        private bool _isClosing;

        public void Init(CalendarManager calendarManager)
        {
            _calendarManager = calendarManager;
            canvasGroup.alpha = 0f;
        }
        
        public override void OnOpenWindow()
        {
            const float timeFadeIn = 0.2f;
            base.OnOpenWindow();

            layoutGroup.spacing = -120f;
            
            _seq?.Kill();
            _seq = DOTween.Sequence();
            _seq.Append(DOVirtual.Float(-120f, 20f, timeFadeIn, v => layoutGroup.spacing = v).SetEase(Ease.OutQuad))
                .Join(DOVirtual.Float(0f, 1f, timeFadeIn, v => canvasGroup.alpha = v));
        }
        
        public override void CloseWindow()
        {
            if (_isClosing) return;
            
            // 閉じる命令が出てすぐ消えるのは不自然なので非同期に投げる
            CloseWindowAsync().Forget();
        }

        [EnumAction(typeof(CalendarType))]
        public void SetCalendarType(int intType)
        {
            var type = (CalendarType)intType;
            
            _calendarManager.SwitchMode(type);
        }

        public void OpenTaskCreationPopup()
        {
            throw new NotImplementedException();
        }

        public void OpenAllDayScheduleCreationPopup()
        {
            var window = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<ScheduleCreationPopup>());
            window.Init(_calendarManager.CurrentTargetDate, true);
        }

        public void OpenScheduleCreationPopup()
        {
            var window = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<ScheduleCreationPopup>());
            window.Init(_calendarManager.CurrentTargetDate, false);
        }

        [EnumAction(typeof(SceneOf))]
        public void MoveScene(int intType)
        {
            var scene = (SceneOf)intType;
            InAppContext.SceneLoader.ChangeScene(scene);
        }

        private async UniTask CloseWindowAsync()
        {
            const float timeFadeOut = 0.1f;

            // 動作中にもう一回動かれたら面倒なので全ての入力をシャットアウト
            _isClosing = true;
            canvasGroup.interactable = false;
            
            // 画面端に動く
            _seq?.Kill();
            _seq = DOTween.Sequence();

            _seq.Append(DOVirtual.Float(1f, 0f, timeFadeOut, v => canvasGroup.alpha = v)
                .SetEase(Ease.InQuad));
            
            // 動き終わるまで待機して、終わったらウィンドウを削除
            await _seq.AsyncWaitForCompletion();
            base.CloseWindow();
        }
    }
}