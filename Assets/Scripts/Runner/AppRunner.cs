using Presentation.Views.Scene;
using UnityEngine;

namespace Runner
{
    public class AppRunner : AdditiveRunner
    {
        private static AppRunner _instance;

        [SerializeField] private SceneLoader sceneLoader;
        
        protected override void Awake()
        {
            // まだAppRunnerが存在していないなら、自分を登録する
            if (!_instance)
            {
                _instance = this;
            }
            // そうでないなら、消える
            else
            {
                Destroy(this);
                return;
            }
        
            Init();
        }

        protected override void Init()
        {
            if (!_instance)
            {
                additiveInstance = this;
                base.Init();
            }
            SceneLoader = sceneLoader;
        }
    }
}
