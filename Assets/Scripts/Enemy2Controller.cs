using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.TakeDamage(transform, 1);
        }
    }
}