using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComfirmPopupData
{
    public string Title;

    public string LeftButtonText;
    public UnityAction OnClickLeftButton;

    public string RightButtonText;
    public UnityAction OnClickRightButton;
}

public class ConfirmPopup : MonoBehaviour, IUIPopup
{
    [SerializeField] private TMP_Text titleText;
    
    [SerializeField] private TMP_Text leftButtonText;
    [SerializeField] private Button leftButton;
    
    [SerializeField] private TMP_Text rightButtonText;
    [SerializeField] private Button rightButton;

    public void Open(object data = null)
    {
        if (data is ComfirmPopupData popupData)
        {
            titleText.text = popupData.Title;
            
            leftButtonText.text = popupData.LeftButtonText;
            leftButton.onClick.AddListener(popupData.OnClickLeftButton);

            rightButtonText.text = popupData.RightButtonText;
            rightButton.onClick.AddListener(popupData.OnClickRightButton);
        }
    }

    public void Close()
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
    }
}
