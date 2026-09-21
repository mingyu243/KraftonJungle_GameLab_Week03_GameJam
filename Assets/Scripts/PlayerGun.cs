using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Settings")]
    [SerializeField] private float aimMoveSpeed;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private TMP_Text bulletCountText;
    [SerializeField] private float shootCooldown = 0.5f;

    [Header("Runtime")]
    [SerializeField] private bool isAiming = false;
    [SerializeField] private float currentshootCooldown;

    public float AimMoveSpeed => aimMoveSpeed;
    public bool IsAiming => isAiming;

    void Start()
    {
        isAiming = false;
        visual.SetActive(false);
        crosshair.SetActive(false);
    }

    void Update()
    {
        if (currentshootCooldown > 0)
        {
            currentshootCooldown -= Time.deltaTime;
        }
    }

    public void StartAiming()
    {
        if (IsAiming)
        {
            return;
        }

        // 조준 시작
        isAiming = true;

        if (InventoryManager.Instance.PlayerInventory.ContainsItem("gun"))
        {
            visual.SetActive(true);
            crosshair.SetActive(true);
        }

        // 남은 탄알 표시
        ItemStack bulletItemStack = InventoryManager.Instance.PlayerInventory.GetItem("bullet");
        if (bulletItemStack != null)
        {
            bulletCountText.text = $"{bulletItemStack.Count}";
        }
        else
        {
            bulletCountText.text = "0";
        }

        CameraManager.Instance.SwitchCamera(CameraType.MainWeaponAim);
    }

    public void CancelAiming()
    {
        // 조준 끝
        isAiming = false;
        visual.SetActive(false);
        crosshair.SetActive(false);
        CameraManager.Instance.SwitchCamera(CameraType.Normal);
    }

    public void Shoot()
    {
        if (InventoryManager.Instance.PlayerInventory.ContainsItem("gun") == false)
        {
            return;
        }

        if (currentshootCooldown > 0)
        {
            return;
        }

        if (InventoryManager.Instance.PlayerInventory.ContainsItem("bullet"))
        {
            visual.SetActive(true);
            crosshair.SetActive(true);

            // 카메라 중앙으로 레이캐스트 쏴서 맞추기
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, targetLayer))
            {
                Debug.Log($"Hit: {hit.collider.name}");

                if (hit.collider.TryGetComponent<Enemy1Controller>(out Enemy1Controller enemy1Controller))
                {
                    Destroy(enemy1Controller.gameObject);
                }
                else if (hit.collider.TryGetComponent<Enemy2Controller>(out Enemy2Controller enemy2Controller))
                {
                    Destroy(enemy2Controller.gameObject);
                }
            }

            // 카메라 impulseSource 로 위로 한번 탕 튀어주기
            impulseSource.GenerateImpulse(Vector3.up * 0.25f);

            currentshootCooldown = shootCooldown;
            InventoryManager.Instance.PlayerInventory.UseItem("bullet");

            // 남은 탄알 표시
            ItemStack bulletItemStack = InventoryManager.Instance.PlayerInventory.GetItem("bullet");
            if (bulletItemStack != null)
            {
                bulletCountText.text = $"{bulletItemStack.Count}";
            }
            else
            {
                bulletCountText.text = "0";
            }
        }
    }
}
