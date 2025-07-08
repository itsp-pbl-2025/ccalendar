using Presentation.Views.Popup;
using Presentation.Views.Popup.Task;
using UnityEngine;

namespace Presentation.Views.Scene.Task
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
