using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Vector3 forceDir = new Vector3(0, 0.3f, 1f);
    [SerializeField] private float minForce = 3f;
    [SerializeField] private float maxForce = 5f;
    [Space]
    [SerializeField] private float minAfterDelay = 1f;
    [SerializeField] private float maxAfterDelay = 2f;
    [Space]
    [SerializeField] private float minEnemyStunDuration = 1f;
    [SerializeField] private float maxEnemyStunDuration = 2f;
    [Space]
    [SerializeField] private float minPunchDuration = 1f;
    [SerializeField] private float maxPunchDuration = 2f;
    [Space]
    [SerializeField] private Vector3 minTargetPos;
    [SerializeField] private Vector3 maxTargetPos;
    [Space]
    [SerializeField] private float minHitStopDuration = 0.05f;
    [SerializeField] private float maxHitStopDuration = 0.2f;
    [Space]
    [SerializeField] private float maxPower = 3f;
    [Space]
    [SerializeField] private float chargingMoveSpeed = 3f;

    [Header("Runtime")]
    [SerializeField] private float currentPower = 0;
    [SerializeField] private bool isCharging = false;
    [SerializeField] private bool isPunching = false;

    public float PunchMoveSpeed => chargingMoveSpeed;
    public bool UsePunching => isCharging || isPunching;

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
        isCharging = false;
        isPunching = false;
        currentPower = 0f;
        hitEnemies.Clear();
        punchCollider.enabled = false;
        visual.SetActive(false);
    }

    void Update()
    {
        // 게이지 충전
        if (isCharging)
        {
            currentPower += Time.deltaTime;
        }

        // 사이즈 조절
        transform.localScale = CurrentScale;
    }

    private HashSet<Enemy1Controller> hitEnemies = new();
    private void OnTriggerStay(Collider other)
    {
        if (!isPunching)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<Enemy1Controller>(out Enemy1Controller enemyController))
        {
            // 중복이면 무시
            if (hitEnemies.Contains(enemyController))
            {
                return;
            }

            hitEnemies.Add(enemyController);

            enemyController.ApplyKnockback(CurrentForce);

            //float currentHitStopDuration = Mathf.Lerp(minHitStopDuration, maxHitStopDuration, t);
            //UniTask.Void(async () =>
            //{
            //    punchCollider.enabled = false;
            //    Time.timeScale = 0f;

            //    await UniTask.WaitForSeconds(currentHitStopDuration, true);

            //    punchCollider.enabled = true;
            //    Time.timeScale = 1f;
            //});

            // 맞출 때마다 파워 감소
            currentPower *= 0.5f;
        }
    }

    public void StartCharging()
    {
        // 펀치 중이면 안 됨
        if (isPunching)
        {
            return;
        }

        isCharging = true;
        isPunching = false;
        currentPower = 0f;
        hitEnemies.Clear();
        visual.SetActive(true);
    }

    public void ReleasePunch()
    {
        // 차징 중이 아니었으면 안 됨
        if (isCharging == false)
        {
            return;
        }

        isCharging = false;
        isPunching = true;
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

            currentPower = 0f;
            visual.SetActive(false);
        });
    }

    public void EndPunch()
    {
        isCharging = false;
        isPunching = false;
        hitEnemies.Clear();
        punchCollider.enabled = false;
    }
}
