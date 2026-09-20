using DG.Tweening;
using UnityEngine;

public class CartInteractor : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Ride")]
    public bool CanRide;

    [Header("Jump")]
    public bool CanJump;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private float jumpDuration = 0.5f;
    private Tweener currentJumpTween;

    [Header("Runtime")]
    public bool IsRiding;
    public bool IsJumping;
    public Transform CurrentCart;

    private Vector3 prevPosition;

    void LateUpdate()
    {
        if (IsRiding == false)
        {
            return;
        }

        // 변한 값만 플레이어 위치에 계속 더해줌
        Vector3 cartDelta = CurrentCart.position - prevPosition;
        rb.MovePosition(rb.position + cartDelta);

        prevPosition = CurrentCart.position;
    }

    public void EnterCart(Transform cartTr)
    {
        CurrentCart = cartTr;
        prevPosition = CurrentCart.position;

        IsRiding = true;
    }

    public void ExitCart()
    {
        CurrentCart = null;

        IsRiding = false;
    }

    public void JumpToCart(Transform landingPoint)
    {
        if (IsJumping)
        {
            return;
        }

        IsJumping = true;
        rb.isKinematic = true; // 점프 중 물리 영향 차단

        transform.DOJump(landingPoint.position, jumpPower, 1, jumpDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                IsJumping = false;
                rb.isKinematic = false;
            });
    }
}