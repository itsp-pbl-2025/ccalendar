using System.Collections.Generic;
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

        public void SubmitScene(AdditiveScene scene)
        {
            _loadedScenes.Push(scene);
        }

        public void AddScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
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