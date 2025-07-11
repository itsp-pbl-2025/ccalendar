using Presentation.Views.Popup;
using UnityEngine;

namespace Presentation.Views.Scene.Sample
{
    public class SampleSidebarSummoner : MonoBehaviour
    {
        [SerializeField] private SampleSidebarPopup popupPrefab;
        [SerializeField] private AutoPaginator paginator;
        
        public void ShowSidebarPopup()
        {
            var window = PopupManager.Instance.ShowPopup(popupPrefab);
            window.Init(paginator);
        }
    }
}