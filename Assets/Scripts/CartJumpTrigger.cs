using System.Collections.Generic;
using UnityEngine;

public class CartJumpTrigger : MonoBehaviour
{
    [SerializeField] private List<Transform> landingPoints;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<CartInteractor>(out CartInteractor interactor))
        {
            if (interactor.CanJump == false)
            {
                return;
            }

            if (interactor.IsRiding == true || interactor.IsJumping)
            {
                return;
            }

            // 가까운 포인트 계산
            Transform closestTr = landingPoints[0];
            float minDistance = Vector3.Distance(other.transform.position, closestTr.position);
            for (int i = 1; i < landingPoints.Count; i++)
            {
                float distance = Vector3.Distance(other.transform.position, landingPoints[i].position);

                if (distance < minDistance)
                {
                    closestTr = landingPoints[i];
                    minDistance = distance;
                }
            }

            interactor.JumpToCart(closestTr);
        }
    }
}
