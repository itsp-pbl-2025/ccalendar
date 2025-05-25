using UnityEngine;

namespace Presentation.Views.Sample
{
    public class SampleSceneManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] deleteInitial;

        private void Awake()
        {
            foreach (var obj in deleteInitial)
            {
                Destroy(obj);
            }
        }
    }
}