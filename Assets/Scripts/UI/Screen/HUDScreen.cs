using System;
using TMPro;
using UnityEngine;

public class HUDScreen : MonoBehaviour, IUIScreen
{
    [Header("Info")]
    [SerializeField] TMP_Text currentDistanceText;
    [SerializeField] TMP_Text nextShelterMeterText;

    public void Open(object data = null)
    {
    }
    public void Close()
    {
    }

    private void Update()
    {
        currentDistanceText.text = $"{GameManager.Instance.Level_Main.CurrentDistance.ToString("F0")}m";
        nextShelterMeterText.text = $"다음 쉼터 {GameManager.Instance.Level_Main.NextShelterMeter}m";
    }
}
