using AppCore.Interfaces;
using Presentation.Utilities;
using UnityEngine;

namespace Presentation.Presenter
{
    public abstract class InAppContext : MonoBehaviour
    {
        public static IContext Context { get; protected set; }
        
        public static EventDispatcher EventDispatcher { get; protected set; }
    }
}