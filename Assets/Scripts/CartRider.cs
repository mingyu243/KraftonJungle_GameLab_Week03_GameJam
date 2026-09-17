using UnityEngine;

public class CartRider : MonoBehaviour
{
    [SerializeField] private Transform currentCart;

    private Vector3 prevPosition;

    void Start()
    {
        prevPosition = currentCart.position;
    }

    void LateUpdate()
    {
        // 변한 값만 플레이어 위치에 계속 더해줌
        Vector3 cartDelta = currentCart.position - prevPosition;
        transform.position += cartDelta;

        prevPosition = currentCart.position;
    }
}