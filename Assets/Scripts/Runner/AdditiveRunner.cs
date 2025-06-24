using AppCore.UseCases;
using Domain.Enum;
using Infrastructure;
using Presentation.Presenter;
using Presentation.Resources;
using Presentation.Utilities;
using SecureStringStorage;
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
            // データベースの初期化
            var dbPath = System.IO.Path.Combine(Application.persistentDataPath, "AppDatabase.db");
            var dbKey = KeyStorage.Load();
            if (dbKey == null)
            {
                dbKey = PasswordGenerator.Generate();
                KeyStorage.Save(dbKey);
            }
            Context ??= new Context(dbPath, dbKey);
            
            // イベント管理クラスの作成
            EventDispatcher ??= new EventDispatcher();
            
            // プレハブ格納庫の作成
            Prefabs ??= new PrefabBundle(prefabDictionary);
            
            // テーマの設定
            var prevTheme = Context.GetService<HistoryService>().GetHistoryOrDefault<string>(HistoryType.ThemeUsing);
            Theme ??= new ThemePalette(Prefabs.GetThemeOrDefault(prevTheme));
        }

        private void OnApplicationQuit()
        {
            // 前回起動時にアプリバージョンを保存しておくとマイグレーションの役に立つ
            Context.GetService<HistoryService>().UpdateHistory(HistoryType.PreviousAppVersion, Application.version);
        }
    }
}