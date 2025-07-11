using UnityEngine;
using Presentation.Views.Popup;
using UnityEngine.UI;

public class TestYearInputPopup : MonoBehaviour
{
    [SerializeField] private Button openButton;
    [SerializeField] private YearInputPopup yearInputPopupPrefab;

    private void Start()
    {
        //
    }

    public void OpenPopup()
    {
        var popup = PopupManager.Instance.ShowPopup(yearInputPopupPrefab);
        popup.Init(OnYearSelected, 2025);
    }

    private void OnYearSelected(int selectedYear)
    {
        Debug.Log($"✅ 年が選択されました: {selectedYear}");
    }
}