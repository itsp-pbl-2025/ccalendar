using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Presentation.Presenter;
using Presentation.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.Views.Scene
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        
        public Camera Camera => mainCamera;
        
        private readonly Stack<AdditiveScene> _loadedScenes = new();
        
        public SceneTransition NextTransition { get; private set; }
        
        public bool BaseSceneReady { get; private set; }

        private void Awake()
        {
            CheckAppLoaded().Forget();
        }

        private async UniTask CheckAppLoaded()
        {
            while (true)
            {
                if ((InAppContext.Context?.Ready ?? false) && InAppContext.SceneLoader) break;
                await UniTask.Yield();
            }
            
            BaseSceneReady = true;
            InAppContext.EventDispatcher.SendGlobalEvent(GlobalEvent.OnAppLoaded);
        }

        public void SubmitScene(AdditiveScene scene)
        {
            _loadedScenes.Push(scene);
        }

        public bool AddScene(string sceneName)
        {
            if (CheckSceneExist(sceneName)) return false;
            
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            return true;
        }

        public bool CheckSceneExist(string sceneName)
        {
            foreach (var scene in _loadedScenes)
            {
                if (scene.SceneName == sceneName) return true;
            }

            return false;
        }

        public void UnloadAdditiveScene(AdditiveScene scene)
        {
            SceneManager.UnloadSceneAsync(scene.SceneName);
        }

        public void ChangeFrontScene(string sceneName, SceneTransition nextTransition)
        {
            NextTransition = nextTransition;
            
            var unloadScene = _loadedScenes.Pop();
            unloadScene.OnSceneUnload();
            
            AddScene(sceneName);
        }
    }
}