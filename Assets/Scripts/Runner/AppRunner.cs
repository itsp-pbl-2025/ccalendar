using AppCore.UseCases;
using Domain.Enum;
using Infrastructure;
using Presentation.Presenter;
using Presentation.Resources;
using Presentation.Utilities;
using UnityEngine;

namespace Runner
{
    public class AppRunner : InAppContext
    {
        private static AppRunner _instance;

        [SerializeField] private PrefabDictionary prefabDictionary;
        
        private void Awake()
        {
            if (ReferenceEquals(_instance, null))
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        
            Context = new Context(System.IO.Path.Combine(Application.persistentDataPath, "AppDatabase.db"));
            EventDispatcher = new EventDispatcher();
            Prefabs = new PrefabBundle(prefabDictionary);
            Theme = new ThemePalette(Prefabs.GetThemeByName(Context.GetService<HistoryService>().GetHistoryOrDefault<string>(HistoryType.ThemeUsing)));
        }

        private void OnApplicationQuit()
        {
            // 前回起動時にアプリバージョンを保存しておくとマイグレーションの役に立つ
            Context.GetService<HistoryService>().UpdateHistory(HistoryType.PreviousAppVersion, Application.version);
        }
    }
}
