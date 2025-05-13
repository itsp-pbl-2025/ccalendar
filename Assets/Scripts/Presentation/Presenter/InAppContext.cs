using AppCore.Interfaces;
using UnityEngine;

namespace Presentation.Presenter
{
    public abstract class InAppContext : MonoBehaviour
    {
        public static IContext Context { get; protected set; }
    }
}