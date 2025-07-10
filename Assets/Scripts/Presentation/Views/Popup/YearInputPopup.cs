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
        [SerializeField] private Button positiveButton, negativeButton;
        [SerializeField] private Button yearLabelButton; // 中央のLabeledRoundButton
        [SerializeField] private Button yearDecrementButton, yearIncrementButton;
        [SerializeField] private Button frameBackgroundButton; // CommonPopupFrame の背景

        private Action<int> _onYearDefined;
        private int _currentYear;
        private bool _isInputMode;
        
        private void Start()
        {
            // 
        }
        
        public void Init(Action<int> onYearDefined, int? initialYear = null)
        {
            _onYearDefined = onYearDefined;
            _currentYear = initialYear ?? DateTime.Now.Year;

            yearInputField.characterLimit = 4;
            yearInputField.contentType = TMP_InputField.ContentType.IntegerNumber;

            SetYear(_currentYear);
            SetupListeners();
            SetInputMode(false);
        }

        private bool _listenersSetup = false;
        private void SetupListeners()
        {
            //二重登録防止
            if (_listenersSetup) return;
            _listenersSetup = true;
            
            positiveButton.onClick.AddListener(OnPressDefineButton);
            negativeButton.onClick.AddListener(CloseWindow);
            yearDecrementButton.onClick.AddListener(() => ChangeYear(-1));
            yearIncrementButton.onClick.AddListener(() => ChangeYear(1));
            yearLabelButton.onClick.AddListener(() => SetInputMode(true));
            frameBackgroundButton.onClick.AddListener(() => { if (_isInputMode) ApplyInputAndReturn(); });
        }

        private void SetYear(int year)
        {
            _currentYear = Mathf.Clamp(year, 1, 9999);
            yearLabel.text = _currentYear.ToString();
            yearInputField.text = _currentYear.ToString();
        }

        private void ChangeYear(int delta)
        {
            SetYear(_currentYear + delta);
        }

        private void SetInputMode(bool inputMode)
        {
            _isInputMode = inputMode;
            inputFieldContainer.SetActive(inputMode);
            yearLabel.gameObject.SetActive(!inputMode);
        }

        private void ApplyInputAndReturn()
        {
            if (int.TryParse(yearInputField.text, out int inputYear))
            {
                SetYear(inputYear);
            }
            SetInputMode(false);
        }

        private bool _isSubmitted = false;
        private void OnPressDefineButton()
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
