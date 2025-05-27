using Presentation.Views.Popup;
using UnityEngine;

namespace Presentation.Views
{
    public class TaskCreationPopupLauncher : MonoBehaviour
    {
        [SerializeField] private TaskCreationPopup popupWindow;
        
        public void OnLaunch()
        {
            PopupManager.Instance.ShowPopup(popupWindow);
        }
    }
}
