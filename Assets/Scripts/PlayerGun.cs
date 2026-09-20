using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private GameObject visual;

    [Header("Settings")]
    [SerializeField] private float aimMoveSpeed;

    [Header("Runtime")]
    [SerializeField] private bool isAiming = false;

    public float AimMoveSpeed => aimMoveSpeed;
    public bool IsAiming => isAiming;

    public void StartAiming()
    {
        if (IsAiming)
        {
            return;
        }

        // 조준 시작
        isAiming = true;
        visual.SetActive(true);
        CameraManager.Instance.SwitchCamera(CameraType.MainWeaponAim);

        // 총 쏘기
        Vector3 forward = CameraManager.Instance.CameraTr.forward;

        // 총 이펙트

        // 카메라 중앙으로 레이캐스트 쏴서 맞추기
    }

    public void CancelAiming()
    {
        // 조준 끝
        isAiming = false;
        visual.SetActive(false);
        CameraManager.Instance.SwitchCamera(CameraType.Normal);
    }

    public void Shoot()
    {

    }
}
