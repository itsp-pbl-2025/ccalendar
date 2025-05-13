using AppCore.UseCases;
using Presentation.Presenter;
using TMPro;
using UnityEngine;

namespace Presentation.Views.Sample
{
    public class SampleDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI sampleText;
        
        public void SetString(string text)
        {
            InAppContext.Context.GetService<SampleService>("ScheduleService");
        }
    }
}