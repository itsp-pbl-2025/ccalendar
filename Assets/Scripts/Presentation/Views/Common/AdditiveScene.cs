using Cysharp.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using Presentation.Presenter;
using UnityEngine.SceneManagement;
#endif

namespace Presentation.Views.Common
{
    public class AdditiveScene : MonoBehaviour
    {
        [SerializeField] private string sceneName;
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
            if (InAppContext.SceneLoader.NextTransition is null)
            {
                canvas.CanvasGroup.alpha = 1f;
                canvas.CanvasRect.sizeDelta = Vector2.zero;
                return;
            }
            
            // Transition along SceneLoader.NextTransition
        }

        public void OnSceneUnload()
        {
            // Transition along SceneLoader.NextTransition
        }
    }
}