using System;
using System.Collections.Generic;
using Presentation.Views.Extensions;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

namespace Presentation.Views.Popup
{
    public class EnumSelectPopup : PopupWindow
    {
        [SerializeField] private RectTransform windowBaseRect;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private LabelRx titleLabel;
        [SerializeField] private ToggleWithLabel prefabToggle;
        
        private Action<int> _closingCallback;
        private int _currentSelecting;
        
        public void Init<T>(string title, Action<T> callback, List<T> enumOrder = null, Dictionary<T, string> enumLabel = null, T selectingFirst = default) where T : Enum
        {
            if (string.IsNullOrEmpty(title))
            {
                titleLabel.gameObject.SetActive(false);               
            }
            else
            {
                titleLabel.text = title;
            }
            
            _closingCallback = option =>
            {
                callback?.Invoke((T)(object)option);
            };

            // ラベルが未定義ならEnumをそのまま表示文字列にしちゃう
            if (enumLabel is null)
            {
                enumLabel = new Dictionary<T, string>();
                foreach (T type in Enum.GetValues(typeof(T)))
                {
                    enumLabel.Add(type, type.ToString());
                }
            }

            // 順序リストが未定義ならラベルからそのまま作っちゃう
            if (enumOrder is null)
            {
                enumOrder = new List<T>();
                foreach (var type in enumLabel.Keys)
                {
                    enumOrder.Add(type);
                }
            }
            
            // トグルの追加
            Dictionary<T, ToggleWithLabel> toggles = new();
            foreach (var type in enumOrder.AsValueEnumerable().Reverse())
            {
                var toggle = Instantiate(prefabToggle, verticalLayoutGroup.transform);
                toggle.gameObject.SetActive(true);
                // 順序リスト内に未定義ラベルがあったらEnumをそのまま表示文字列にしちゃう
                toggle.Label.text = enumLabel.TryGetValue(type, out var label) ? label : type.ToString();
                toggle.Toggle.onValueChanged.AddListener(b =>
                {
                    if (b) OnValueSelected((int)(object)type);
                });
                toggles.Add(type, toggle);
            }

            // 初期入力
            if (toggles.TryGetValue(selectingFirst, out var selecting))
            {
                selecting.Toggle.isOn = true;
            }
            else
            {
                toggles[enumOrder[0]].Toggle.isOn = true;
            }

            if (!string.IsNullOrEmpty(title))
            {
                titleLabel.transform.parent.SetAsLastSibling();
            }
            
            // 大きさ調整
            if (verticalLayoutGroup.transform is RectTransform tf)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(tf);
                Canvas.ForceUpdateCanvases();
            }
            windowBaseRect.sizeDelta = new Vector2(windowBaseRect.sizeDelta.x, verticalLayoutGroup.preferredHeight);
        }

        private void OnValueSelected(int value)
        {
            _currentSelecting = value;
        }

        public void CloseWithDefine()
        {
            _closingCallback?.Invoke(_currentSelecting);
            CloseWindow();
        }
    }
}