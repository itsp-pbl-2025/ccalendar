using AppCore.UseCases;
using Domain.Enum;
using Infrastructure;
using Presentation.Presenter;
using Presentation.Resources;
using Presentation.Utilities;
using UnityEngine;

namespace Runner
{
    public class AdditiveRunner : InAppContext
    {
        protected static AdditiveRunner additiveInstance;
        
        [SerializeField] private PrefabDictionary prefabDictionary;
        
        protected virtual void Awake()
        {
            // まだAdditiveRunnerが存在していないなら、自分を登録する
            if (!additiveInstance)
            {
                additiveInstance = this;
            }
            // そうでないなら、自分自身を消す
            else
            {
                Destroy(this);
                return;
            }
        
            Init();
        }

        protected virtual void Init()
        {
            Context ??= new Context(System.IO.Path.Combine(Application.persistentDataPath, "AppDatabase.db"));
            EventDispatcher ??= new EventDispatcher();
            Prefabs ??= new PrefabBundle(prefabDictionary);
            Theme ??= new ThemePalette(Prefabs.GetThemeByName(Context.GetService<HistoryService>().GetHistoryOrDefault<string>(HistoryType.ThemeUsing)));
        }

        private void OnApplicationQuit()
        {
            // 前回起動時にアプリバージョンを保存しておくとマイグレーションの役に立つ
            Context.GetService<HistoryService>().UpdateHistory(HistoryType.PreviousAppVersion, Application.version);
        }
    }
}