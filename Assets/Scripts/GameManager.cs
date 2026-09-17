using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int CurrentStageNumber;
    public int CurrentPlayerHealth; // 정상 (Fine), 주의 (Caution), 위험 (Danger) 

    void Awake()
    {
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}