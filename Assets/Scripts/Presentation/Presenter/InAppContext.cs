using AppCore.Interfaces;
using Presentation.Resources;
using Presentation.Utilities;
using Presentation.Views.Scene;
using UnityEngine;

namespace Presentation.Presenter
{
    public abstract class InAppContext : MonoBehaviour
    {
        public static IContext Context { get; protected set; }
        
        public static EventDispatcher EventDispatcher { get; protected set; }
        
        public static PrefabBundle Prefabs { get; protected set; }
        
        public static SceneLoader SceneLoader { get; protected set; }
        
        public static ThemePalette Theme { get; protected set; }
        
#if UNITY_EDITOR
        public static bool IsDebug { get; private set; }

        public static void SetDebugFlag()
        {
            IsDebug = true;
        }
#else
        // 万が一、誤ってUNITY_EDITOR外で呼び出してしまった時のために常にfalseを返すプロパティを用意しておく
        public static bool IsDebug => false;
#endif
    }
}
