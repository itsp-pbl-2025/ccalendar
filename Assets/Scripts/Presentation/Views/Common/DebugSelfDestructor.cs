using UnityEngine;

namespace Presentation.Views.Common
{
    public class DebugSelfDestructor : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}