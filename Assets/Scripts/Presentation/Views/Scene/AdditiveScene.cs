﻿using System;
using Cysharp.Threading.Tasks;
using Presentation.Presenter;
using Presentation.Views.Common;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace Presentation.Views.Scene
{
    public class AdditiveScene : MonoBehaviour
    {
        [SerializeField] private SceneOf scene;
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private AutoAspectCanvas canvas;
        [SerializeField] private CanvasTransitioner transitioner;

        public SceneOf Scene => scene;
        
        private void Awake()
        {
            transitioner.HideCanvasFast();
            
#if UNITY_EDITOR
            if (!InAppContext.SceneLoader)
            {
                DebugAwake().Forget();
            }
            else
            {
                OnSceneLoaded();
            }
#else
            OnSceneLoaded();
#endif
        }

#if UNITY_EDITOR
        private async UniTask DebugAwake()
        {
            InAppContext.SetDebugFlag();
            await SceneManager.LoadSceneAsync(SceneOf.Base.ToName(), LoadSceneMode.Additive);
            
            // AppRunnerがInAppContextを完全にロードして完了するまで待機する
            while (true)
            {
                if (InAppContext.SceneLoader?.BaseSceneReady ?? false) break;
                await UniTask.Yield();
            }
            
            transitioner.ShowCanvasFast();
            OnSceneLoaded();
        }
#endif

        private void OnSceneLoaded()
        {
            sceneCamera.enabled = false;
            canvas.Canvas.worldCamera = InAppContext.SceneLoader.Camera;
            
            InAppContext.SceneLoader.SubmitScene(this);
        }

        public void SceneTransitionIn(SceneTransition st, Action completeCallback = null)
        {
            if (st is null)
            {
                transitioner.ShowCanvasFast();
                completeCallback?.Invoke();
                return;
            }

            transitioner.ShowCanvasWithAnimation(st, completeCallback);
        }

        public void SceneTransitionOut(SceneTransition st, Action completeCallback = null)
        {
            if (st is null)
            {
                transitioner.HideCanvasFast();
                completeCallback?.Invoke();
                return;
            }
            
            transitioner.HideCanvasWithAnimation(st, completeCallback);
        }

        public void OnSceneUnload()
        {
            Debug.Log($"[{scene}] I'm unloading now!");
        }
    }
}