using Presentation.Presenter;
using Presentation.Utilities;
using R3;
using UnityEngine;

namespace Presentation.Views.Scene.Base
{
    public class AppSetupLoader : MonoBehaviour
    {
        private bool _initialized;
        
        private void Awake()
        {
            if (InAppContext.SceneLoader?.BaseSceneReady ?? false)
            {
                LoadInitialScene();
            }
            else
            {
                InAppContext.EventDispatcher.AddGlobalEventListener(this, GlobalEvent.OnAppLoaded, _ =>
                {
                    LoadInitialScene();
                }).AddTo(this);
            }
        }

        private void LoadInitialScene()
        {
            if (_initialized) return;
            _initialized = true;
            
            InAppContext.SceneLoader.AddScene(SceneSettings.SceneSample);
        }
    }
}