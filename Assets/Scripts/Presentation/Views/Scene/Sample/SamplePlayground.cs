using Presentation.Views.Popup;
using UnityEngine;

namespace Presentation.Views.Scene.Sample
{
    public class SamplePlayground : MonoBehaviour
    {
        /// <summary>
        /// ボタンが一つのポップアップを出現させます。
        /// </summary>
        public void ShowSinglePopup()
        {
            PopupManager.Instance.ShowSinglePopup(
                "このフレームは Prefabs/<size=0> </size>Frames/<size=0> </size>Common/<size=0> </size>Popup/<size=0> </size>SingleButtonPopup です。",
                "閉じる",
                null);
        }

        /// <summary>
        /// ボタンが二つのポップアップを、ある程度再帰的に出現させます。
        /// </summary>
        /// <param name="index">何番目か</param>
        public void ShowDoublePopup(int index)
        {
            var message = index switch
            {
                0 => "このポップアップはいくらでも出せます。",
                < 4 => $"これは{index+1}回目のポップアップです。",
                4 => "結構開きますね。",
                < 9 => $"もう{index+1}個ものポップアップが出ました。",
                9 => "何回開くんですか？…",
                _ => $"これで{index+1}個目です。"
            };

            if (index < 20)
            {
                PopupManager.Instance.ShowDoublePopup(
                    message,
                    "次を開く", () => ShowDoublePopup(index + 1),
                    "閉じる", null);
            }
            else
            {
                PopupManager.Instance.ShowSinglePopup("終わりにしましょう。");
            }
        }
    }
}