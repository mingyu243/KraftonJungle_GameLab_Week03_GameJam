using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour, IUIScreen
{
    [SerializeField] private TMP_Text currentDistanceText;

    public void Open(object data = null)
    {
        Time.timeScale = 0f;
        GameManager.Instance.SetVisibleCursor(true);

        currentDistanceText.text = $"기록 : {GameManager.Instance.Level_Main.CurrentDistance.ToString("F0")}m";
    }
    public void Close()
    {
    }

    public void OnClickRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
