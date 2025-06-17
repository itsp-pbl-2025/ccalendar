using Infrastructure;
using Presentation.Presenter;
using Presentation.Resources;
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
            Prefabs = new PrefabBundle(prefabDictionary);
        }
    }
}