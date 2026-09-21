using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum PunchState
{
    None,
    Charging,
    Punch,
    Return
}

public class PlayerPunch : MonoBehaviour
{
    [SerializeField] private GameObject visual;
    [SerializeField] private Collider punchCollider;
    
    [Header("Settings")]
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 3f;
    [Space]
    [SerializeField] private float minForwardRange = 1f;
    [SerializeField] private float maxForwardRange = 2f;
    [Space]
    [SerializeField] private Vector3 forceDir = new Vector3(0, 1f, 1f);
    [SerializeField] private float minForce = 3f;
    [SerializeField] private float maxForce = 10f;
    [Space]
    [SerializeField] private float minAfterDelay = 0.2f;
    [SerializeField] private float maxAfterDelay = 0.4f;
    [Space]
    [SerializeField] private float minEnemyStunDuration = 1f;
    [SerializeField] private float maxEnemyStunDuration = 2f;
    [Space]
    [SerializeField] private float minPunchDuration = 0.15f;
    [SerializeField] private float maxPunchDuration = 0.15f;
    [Space]
    [SerializeField] private Vector3 minTargetPos = new Vector3(-0.5f, 0.3f, 1.5f);
    [SerializeField] private Vector3 maxTargetPos = new Vector3(-0.5f, 0.3f, 2f);
    [Space]
    [SerializeField] private float minHitStopDuration = 0.05f;
    [SerializeField] private float maxHitStopDuration = 0.2f;
    [Space]
    [SerializeField] private float maxPower = 3f;
    [Space]
    [SerializeField] private float chargingMoveSpeed = 3f;
    [Space]
    [SerializeField] private int maxPushCount = 1;

    [Header("Runtime")]
    [SerializeField] private float currentPower = 0;
    [SerializeField] private PunchState punchState = PunchState.None;

    public float PunchMoveSpeed => chargingMoveSpeed;
    public bool UsePunching => (punchState != PunchState.None);
    public PunchState PunchState => punchState;

    public float CurrentPower => Mathf.Clamp(currentPower, 0f, maxPower);
    public float PowerRatio => Mathf.Clamp01(currentPower / maxPower);
    public Vector3 CurrentScale => Vector3.one * Mathf.Lerp(minScale, maxScale, PowerRatio);
    public float CurrentForwardRange => Mathf.Lerp(minForwardRange, maxForwardRange, PowerRatio);
    public Vector3 CurrentForce => transform.TransformDirection(forceDir).normalized * Mathf.Lerp(minForce, maxForce, PowerRatio);
    public float CurrentAfterDelay => Mathf.Lerp(minAfterDelay, maxAfterDelay, PowerRatio);
    public float CurrentEnemyStunDuration => Mathf.Lerp(minEnemyStunDuration, maxEnemyStunDuration, PowerRatio);
    public float CurrentPunchDuration => Mathf.Lerp(minPunchDuration, maxPunchDuration, PowerRatio);
    public Vector3 CurrentTargetPos => Vector3.Lerp(minTargetPos, maxTargetPos, PowerRatio);
    public float CurrentHitStopDuration => Mathf.Lerp(minHitStopDuration, maxHitStopDuration, PowerRatio);


    void Start()
    {
        punchState = PunchState.None;
        currentPower = 0f;
        hitEnemies.Clear();
        punchCollider.enabled = false;
        visual.SetActive(false);
    }

    void Update()
    {
        // 게이지 충전
        if (punchState == PunchState.Charging)
        {
            currentPower += Time.deltaTime;

        }

        // 사이즈 조절
        transform.localScale = CurrentScale;
    }

    private HashSet<Enemy1Controller> hitEnemies = new();
    private void OnTriggerStay(Collider other)
    {
        if (punchState != PunchState.Punch)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<Enemy1Controller>(out Enemy1Controller enemyController))
        {
            // 한번에 밀 수 있는 개수
            if (hitEnemies.Count >= maxPushCount)
            {
                return;
            }

            // 중복이면 무시
            if (hitEnemies.Contains(enemyController))
            {
                return;
            }

            hitEnemies.Add(enemyController);

            enemyController.ApplyKnockback(CurrentForce);
        }
    }

    public void StartCharging()
    {
        // 펀치 중이면 안 됨
        if (UsePunching)
        {
            return;
        }

        punchState = PunchState.Charging;
        currentPower = 0f;
        hitEnemies.Clear();
        visual.SetActive(true);
    }

    public void ReleasePunch()
    {
        // 차징 중이 아니었으면 안 됨
        if (punchState != PunchState.Charging)
        {
            return;
        }

        punchState = PunchState.Punch;
        hitEnemies.Clear();
        punchCollider.enabled = true;

        GameManager.Instance.PlayerController.LockRotation = true;

        Sequence punchSequence = DOTween.Sequence();
        punchSequence.Append(transform.DOLocalMove(CurrentTargetPos, CurrentPunchDuration).SetEase(Ease.OutQuart));
        punchSequence.AppendCallback(EndPunch);
        punchSequence.AppendInterval(CurrentAfterDelay);
        punchSequence.Append(transform.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.InQuad));
        punchSequence.OnComplete(() =>
        {
            GameManager.Instance.PlayerController.LockRotation = false;
            punchState = PunchState.None;
            currentPower = 0f;
            visual.SetActive(false);
        });
    }

    public void EndPunch()
    {
        punchState = PunchState.Return;
        hitEnemies.Clear();
        punchCollider.enabled = false;
    }
}
