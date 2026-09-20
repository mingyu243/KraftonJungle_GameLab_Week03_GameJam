using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Enemy1Controller : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CartInteractor cartInteractor;
    [SerializeField] private float moveSpeed = 3f;

    public bool IsKnockedBack = false;

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

        // 이동, 회전
        Vector3 pos = rb.position + (dir * moveSpeed * Time.fixedDeltaTime);
        Quaternion rot = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(dir), 10f * Time.fixedDeltaTime);
        rb.Move(pos, rot);
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