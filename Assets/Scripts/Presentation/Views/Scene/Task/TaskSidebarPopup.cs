using System;
using AppCore.UseCases;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain.Entity;
using Presentation.Presenter;
using Presentation.Views.Extensions;
using Presentation.Views.Popup;
using Presentation.Views.Popup.Task;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Scene.Task
{
    public class TaskSidebarPopup : PopupWindow
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        
        private TaskListViewer _taskManager;
        private Sequence _seq;
        private bool _isClosing;
        
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

        public void OpenTaskCreationPopup()
        {
            var window = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<TaskCreationPopup>());
        }

        public void ExecuteAutoFill()
        {
            var t2s = InAppContext.Context.GetService<Task2ScheduleService>();
            var task = InAppContext.Context.GetService<TaskService>();
            var now = CCTimeOnly.Now;
            var start = new CCDateTime(CCDateOnly.Today, new CCTimeOnly(now.Hour.Value, 0, 0).AddHours(1));
            
            t2s.GenerateSchedule(task.GetTask(), start);
        }

        [EnumAction(typeof(SceneOf))]
        public void MoveScene(int intType)
        {
            var scene = (SceneOf)intType;
            InAppContext.SceneLoader.ChangeScene(scene);
        }
        
        public override void CloseWindow()
        {
            if (_isClosing) return;
            
            // 閉じる命令が出てすぐ消えるのは不自然なので非同期に投げる
            CloseWindowAsync().Forget();
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