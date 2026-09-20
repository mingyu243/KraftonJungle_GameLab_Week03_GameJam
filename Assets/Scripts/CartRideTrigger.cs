using UnityEngine;

public class CartRideTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CartInteractor>(out CartInteractor interactor))
        {
            if (interactor.CanRide == false)
            {
                return;
            }
            
            interactor.EnterCart(this.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<CartInteractor>(out CartInteractor interactor))
        {
            interactor.ExitCart();
        }
    }
}
