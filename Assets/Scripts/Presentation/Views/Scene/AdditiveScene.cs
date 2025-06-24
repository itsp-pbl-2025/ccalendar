using Cysharp.Threading.Tasks;
using Presentation.Views.Common;
using UnityEngine;

#if UNITY_EDITOR
using Presentation.Presenter;
using UnityEngine.SceneManagement;
#endif

namespace Presentation.Views.Scene
{
    public class AdditiveScene : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private AutoAspectCanvas canvas;
        [SerializeField] private CanvasTransitioner transitioner;

        public string SceneName => sceneName;
        
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
            await SceneManager.LoadSceneAsync(SceneSettings.SceneBase, LoadSceneMode.Additive);
            
            // AppRunnerがInAppContextを完全にロードして完了するまで待機する
            while (true)
            {
                if (InAppContext.SceneLoader?.BaseSceneReady ?? false) break;
                await UniTask.Yield();
            }
            
            OnSceneLoaded();
        }
#endif

        private void OnSceneLoaded()
        {
            sceneCamera.enabled = false;
            canvas.Canvas.worldCamera = InAppContext.SceneLoader.Camera;
            
            if (InAppContext.SceneLoader.NextTransition is null)
            {
                transitioner.ShowCanvasFast();
            }
            else
            {
                transitioner.ShowCanvasWithAnimation(InAppContext.SceneLoader.NextTransition, null);
            }
            
            InAppContext.SceneLoader.SubmitScene(this);
        }

        public void OnSceneUnload()
        {
            if (InAppContext.SceneLoader.NextTransition is null)
            {
                transitioner.HideCanvasFast();
                return;
            }
            
            transitioner.HideCanvasWithAnimation(InAppContext.SceneLoader.NextTransition, null);
        }
    }
}