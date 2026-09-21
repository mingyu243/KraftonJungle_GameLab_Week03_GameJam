using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class Enemy1Controller : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private CartInteractor cartInteractor;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackDuration; // 공격
    [SerializeField] private float attackChargeDuration; // 공격 차징
    [SerializeField] private float minAfterDelay = 1.5f;
    [SerializeField] private float maxAfterDelay = 3f;
    [SerializeField] private float minAttackCooldown = 1.5f; // 최소 쿨타임
    [SerializeField] private float maxAttackCooldown = 3f; // 최대 쿨타임
    private float currentAfterDelay = 0f;
    private float currentCooldownTimer = 0f;
    private Sequence currentAttackSequence;

    [Header("Attack Range")]
    [SerializeField] private GameObject attackRangeVisual;
    private Material attackRangeVisualMaterial;

    [Header("Runtime")]
    public bool IsKnockedBack = false;
    public bool IsAttacking = false;
    public float CurrentGauge;

    void Awake()
    {
        attackRangeVisualMaterial = attackRangeVisual.GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        attackRangeVisual.SetActive(false);
        enemyCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (IsKnockedBack)
        {
            return;
        }

        Transform targetTr = GameManager.Instance.PlayerController.transform;

        // 방향
        Vector3 dir = (targetTr.position - transform.position);
        dir.y = 0;
        dir.Normalize();

        // 카트 위에 있다면
        if (cartInteractor.IsRiding && cartInteractor.IsJumping == false && IsAttacking == false)
        {
            // 플레이어가 공격 사거리 안에 들어오면
            float distance = Vector3.Distance(transform.position, targetTr.position);
            if (distance <= attackRange && currentCooldownTimer <= 0f)
            {
                // 공격을 시작할 때 쿨타임 미리 세팅
                currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown);

                currentAttackSequence = DOTween.Sequence();

                Tween dashTween = null;

                // 공격 차징
                currentAttackSequence.Append(
                    DOTween.To(
                        () => 0f, x => 
                        {
                            attackRangeVisualMaterial.SetFloat("_ProgressRatio", x);
                        },
                        1f,
                        attackChargeDuration
                    )
                    .SetEase(Ease.Linear)
                    .OnStart(() =>
                    {
                        IsAttacking = true;
                        attackRangeVisual.SetActive(true);

                        Vector3 pos = targetTr.position;
                        pos.y = transform.position.y;
                        transform.LookAt(pos);
                    })
                    .OnComplete(() =>
                    {
                        attackRangeVisual.SetActive(false);
                    })
                );

                // 돌진
                currentAttackSequence.Append(
                    DOVirtual.Float(0f, 1f, attackDuration, (x) => { }) // 빈 타이머 역할
                    .OnStart(() =>
                    {
                        enemyCollider.enabled = true;

                        // 돌진하는 순간 포지션이 계산되어야 해서 이렇게 씀
                        dashTween = transform.DOMove(transform.position + transform.forward * attackRange, attackDuration)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                IsAttacking = false;
                                enemyCollider.enabled = false;
                                currentAttackSequence = null;
                            });
                    })
                );

                currentAttackSequence.OnKill(() =>
                {
                    dashTween?.Kill();

                    IsAttacking = false;
                    attackRangeVisual.SetActive(false);
                    enemyCollider.enabled = false;
                    currentAttackSequence = null;
                });
            }
        }

        // 공격하는 중이 아니면
        if (IsAttacking == false)
        {
            // 공격 쿨타임 계산
            if (currentCooldownTimer > 0)
            {
                currentCooldownTimer -= Time.fixedDeltaTime;
            }
            else
            {
                // 공격이 가능할 때만
                // 이동, 회전
                Vector3 pos = rb.position + (dir * moveSpeed * Time.fixedDeltaTime);
                Quaternion rot = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(dir), 10f * Time.fixedDeltaTime);
                rb.Move(pos, rot);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.TakeDamage(transform, 1);
        }
    }

    public void CancelAttack()
    {
        currentAttackSequence?.Kill();
    }

    private CancellationTokenSource cts;

    public void ApplyKnockback(Vector3 force)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();

        ApplyKnockbackAsync(force, cts.Token).Forget();
    }

    private async UniTaskVoid ApplyKnockbackAsync(Vector3 force, CancellationToken token)
    {
        try
        {
            IsKnockedBack = true;
            cartInteractor.CanJump = false;

            CancelAttack();
            cartInteractor.CancelJump();

            rb.linearVelocity = Vector3.zero;
            rb.AddForce(force, ForceMode.Impulse);

            await UniTask.WaitForSeconds(1.6f, cancellationToken: token);

            cartInteractor.CanJump = true;
            IsKnockedBack = false;
        }
        catch
        {
        }
    }
}