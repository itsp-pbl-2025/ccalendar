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
        
        private readonly HashSet<SceneOf> _keepScenes = new();
        private readonly Dictionary<SceneOf, AdditiveScene> _loadedScenes = new();
        private readonly Dictionary<SceneOf, bool> _transitionWaiting = new();
        private SceneOf _currentScene;

        private SceneTransition _nextTransition;
        private bool _unloadWithTransition;
        
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

        /// <summary>
        /// ロードが完了したAdditiveSceneが自分自身を報告するメソッド。
        /// AdditiveScene.OnSceneLoaded()以外で呼ばないこと。
        /// </summary>
        /// <param name="scene">自分自身を報告</param>
        public void SubmitScene(AdditiveScene scene)
        {
            _loadedScenes.Add(scene.Scene, scene);
            if (_transitionWaiting.Remove(scene.Scene, out var transition))
            {
                if (transition) PlaySceneTransition(scene.Scene, _currentScene, _nextTransition);
            }
            _nextTransition = null;
        }

        /// <summary>
        /// シーンを追加するだけして表示しない。
        /// </summary>
        /// <param name="scene">追加するシーン</param>
        public void PushScene(SceneOf scene)
        {
            if (CheckSceneExist(scene)) return;
            _transitionWaiting.Add(scene, false);
            SceneManager.LoadSceneAsync(scene.ToName(), LoadSceneMode.Additive);
        }

        /// <summary>
        /// シーンを変化させる。
        /// </summary>
        /// <param name="scene">次に表示するシーン</param>
        /// <param name="sceneTransition">シーンの変化演出</param>
        /// <param name="unload">出ていったシーンが退出した後アンロードするか</param>
        public void ChangeScene(SceneOf scene, SceneTransition sceneTransition = null, bool unload = false)
        {
            // 無効なシーンか、既にそのシーンにいたらキャンセル
            if (scene is SceneOf.Base || scene == _currentScene) return;
            
            _unloadWithTransition = unload;
            // キャッシュに存在したら普通に演出
            if (CheckSceneExist(scene))
            {
                _keepScenes.Add(scene);
                PlaySceneTransition(scene, _currentScene, sceneTransition);
                return;
            }
            
            // 存在しなかったら生成して待機
            _nextTransition = sceneTransition;
            _transitionWaiting.Add(scene, true);
            SceneManager.LoadSceneAsync(scene.ToName(), LoadSceneMode.Additive);
        }

        /// <summary>
        /// シーンが存在するかどうか
        /// </summary>
        /// <param name="sceneType">確認するシーン</param>
        /// <returns>あるかどうか</returns>
        public bool CheckSceneExist(SceneOf sceneType)
        {
            if (sceneType is SceneOf.Base) return true;
            
            foreach (var (type, _) in _loadedScenes)
            {
                if (type == sceneType) return true;
            }

            return false;
        }

        private void PlaySceneTransition(SceneOf from, SceneOf to, SceneTransition st)
        {
            if (_loadedScenes.TryGetValue(from, out var fromScene))
            {
                fromScene.SceneTransitionIn(st, () => _keepScenes.Remove(from));
            }

            if (_loadedScenes.TryGetValue(to, out var toScene))
            {
                if (_unloadWithTransition)
                {
                    toScene.SceneTransitionOut(st, () => UnloadAdditiveScene(to));
                }
                else
                {
                    toScene.SceneTransitionOut(st);
                }
            }
        }

        /// <summary>
        /// 現在表示されているシーンと演出待ちのシーンを残して他のAdditiveSceneを全てアンロードする
        /// </summary>
        public void UnloadWithoutCurrentScene()
        {
            foreach (var (type, additive) in _loadedScenes)
            {
                if (_keepScenes.Contains(type)) continue;
                if (type is SceneOf.Base || type == _currentScene) continue;
                additive.OnSceneUnload();
                SceneManager.UnloadSceneAsync(type.ToName());
            }
        }

        /// <summary>
        /// シーンを指定してアンロードする
        /// </summary>
        /// <param name="scene">アンロードするシーン</param>
        /// <returns>できたかどうか</returns>
        public bool UnloadAdditiveScene(SceneOf scene)
        {
            if (_keepScenes.Contains(scene)) return false;
            if (!_loadedScenes.TryGetValue(scene, out var additive)) return false;
            
            additive.OnSceneUnload();
            SceneManager.UnloadSceneAsync(scene.ToName());
            return true;
        }
    }
}
