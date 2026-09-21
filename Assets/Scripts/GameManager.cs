using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Level_Main Level_Main;
    public PlayerController PlayerController;

    void Awake()
    {
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetVisibleCursor(false);
    }

    public void SetVisibleCursor(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}