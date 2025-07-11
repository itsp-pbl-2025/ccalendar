using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Popup
{
    public class YearInputPopup : PopupWindow
    {
        [SerializeField] private GameObject inputFieldContainer; // 入力モード用のオブジェクト
        [SerializeField] private TMP_InputField yearInputField;
        [SerializeField] private TextMeshProUGUI yearLabel;

        private Action<int> _onYearDefined;
        private int _currentYear;
        private bool _isInputMode;
        private bool _isSubmitted = false;
        
        private void Start()
        {
            // 
        }
        
        public void Init(Action<int> onYearDefined, int? initialYear = null)
        {
            _onYearDefined = onYearDefined;
            _currentYear = initialYear ?? DateTime.Now.Year;


            SetYear(_currentYear);
            SetInputMode(false);
            _isSubmitted = false;
        }
        
        public void OnDecrementYear() => ChangeYear(-1);
        public void OnIncrementYear() => ChangeYear(1);
        public void OnLabelClick() => SetInputMode(true);
        public void OnBackgroundClick()
        {
            if (_isInputMode) ApplyInputAndReturn();
        }
        

        private void SetYear(int year)
        {
            _currentYear = Mathf.Clamp(year, 1, 9999);
            yearLabel.text = _currentYear.ToString();
            yearInputField.text = _currentYear.ToString();
        }

        private void ChangeYear(int delta)
        {
            int baseYear;

            // 入力モード中 or 入力直後などに対応
            if (int.TryParse(yearInputField.text, out int parsed))
            {
                baseYear = parsed;
            }
            else
            {
                baseYear = _currentYear;
            }

            SetYear(baseYear + delta);
        }

        private void SetInputMode(bool inputMode)
        {
            _isInputMode = inputMode;
            inputFieldContainer.SetActive(inputMode);
            yearLabel.gameObject.SetActive(!inputMode);
            //1クリックで入力できる状態に
            if (inputMode)
            {
                // InputFieldを有効化し、フォーカスを当てる
                yearInputField.Select();
                yearInputField.ActivateInputField();
            }
        }

        private void ApplyInputAndReturn()
        {
            if (int.TryParse(yearInputField.text, out int inputYear))
            {
                SetYear(inputYear);
            }
            SetInputMode(false);
        }

        
        public void OnPressDefineButton()
        {
            //二重クリック防止
            if (_isSubmitted) return;
            _isSubmitted = true;
            
            if (_isInputMode)
            {
                ApplyInputAndReturn();
            }
            _onYearDefined?.Invoke(_currentYear);
            CloseWindow();
        }
    }
}
