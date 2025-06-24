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

        public string SceneName => sceneName;
        
        private void Awake()
        {
#if UNITY_EDITOR
            DebugAwake().Forget();
#else
            OnSceneLoaded();
#endif
        }

#if UNITY_EDITOR
        private async UniTask DebugAwake()
        {
            await SceneManager.LoadSceneAsync("BaseScene", LoadSceneMode.Additive);
            
            // AppRunnerがInAppContextをロードするまで待機する
            while (true)
            {
                if (InAppContext.Context is not null) break;
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
                canvas.CanvasGroup.alpha = 1f;
                canvas.CanvasRect.sizeDelta = Vector2.zero;
                return;
            }
            
            Debug.Log("Scene Loaded successfully");
            // Transition along SceneLoader.NextTransition
        }

        public void OnSceneUnload()
        {
            // Transition along SceneLoader.NextTransition
        }
    }
}