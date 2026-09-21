using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UIScreenType
{
    None,
    HUD,
    Shelter,
    End
}

public enum UIPopupType
{
    None,
    Inventory,
    Confirm,
}

public interface IUIScreen
{
    void Open(object data = null);
    void Close();
}

public interface IUIPopup
{
    void Open(object data = null);
    void Close();
}

[Serializable]
public class UIScreenBinding
{
    public UIScreenType type;
    public GameObject instance; // 씬에 이미 있는 오브젝트
}

[Serializable]
public class UIPopupBinding
{
    public UIPopupType type;
    public GameObject instance; // 씬에 이미 있는 오브젝트
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Bindings")]
    [SerializeField] private List<UIScreenBinding> screenBindings;
    [SerializeField] private List<UIPopupBinding> popupBindings;

    private Dictionary<UIScreenType, GameObject> screenDict;
    private Dictionary<UIPopupType, GameObject> popupDict;

    void Awake()
    {
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        screenDict = screenBindings.ToDictionary(x => x.type, x => x.instance);
        popupDict = popupBindings.ToDictionary(x => x.type, x => x.instance);

        // 시작할 땐 다 꺼둠
        foreach (var obj in screenDict.Values) obj.SetActive(false);
        foreach (var obj in popupDict.Values) obj.SetActive(false);
    }

    #region Screen

    // 교체 방식의 UI
    private UIScreenType currentScreenType = UIScreenType.None;

    public UIScreenType CurrentScreenType => currentScreenType;

    public void OpenScreen(UIScreenType type, object data = null)
    {
        // 기존 것 끄기
        if (screenDict.TryGetValue(currentScreenType, out var prevObj))
        {
            prevObj.GetComponent<IUIScreen>().Close();
            prevObj.SetActive(false);
        }

        // 새로운 것 켜기
        if (screenDict.TryGetValue(type, out var newObj))
        {
            newObj.SetActive(true);
            newObj.GetComponent<IUIScreen>().Open(data);
            currentScreenType = type;
        }
    }

    #endregion

    #region Popup

    // 스택 방식의 UI
    private List<UIPopupType> popupStack = new();

    public bool IsPopupOpen(UIPopupType type)
    {
        return (popupStack.Contains(type));
    }

    public void OpenPopup(UIPopupType type, object data = null)
    {
        if (popupStack.Contains(type))
        {
            return;
        }

        if (popupDict.TryGetValue(type, out var obj))
        {
            obj.SetActive(true);
            obj.GetComponent<IUIPopup>().Open(data);
            popupStack.Add(type);
        }
    }

    public void CloseTopPopup()
    {
        if (popupStack.Count == 0)
        {
            return;
        }

        var type = popupStack[(popupStack.Count - 1)];
        ClosePopup(type);
    }

    public void ClosePopup(UIPopupType type)
    {
        if (!popupStack.Contains(type))
        {
            return;
        }

        if (popupDict.TryGetValue(type, out var obj))
        {
            obj.GetComponent<IUIPopup>()?.Close();
            obj.SetActive(false);
        }

        popupStack.Remove(type);
    }

    public void CloseAllPopups()
    {
        while (popupStack.Count > 0)
        {
            CloseTopPopup();
        }
    }

    #endregion
}
