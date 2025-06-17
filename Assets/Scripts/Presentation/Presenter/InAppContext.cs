using AppCore.Interfaces;
using Presentation.Resources;
using UnityEngine;

namespace Presentation.Presenter
{
    public abstract class InAppContext : MonoBehaviour
    {
        public static IContext Context { get; protected set; }
        
        public static PrefabBundle Prefabs { get; protected set; }
    }
}