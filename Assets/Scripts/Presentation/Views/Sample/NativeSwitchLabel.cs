using TMPro;
using UnityEngine;

namespace Presentation.Views.Sample
{
    public class NativeSwitchLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        
        [SerializeField] [TextArea(minLines: 3, maxLines: 10)]
        private string editorText, pcText, androidText, iphoneText;

        private void Start()
        {
#if UNITY_EDITOR
            label.text = editorText;
#elif UNITY_STANDALONE
            label.text = pcText;
#elif UNITY_ANDROID
            label.text = androidText;
#elif UNITY_IPHONE
            label.text = iphoneText;
#endif
        }
    }
}