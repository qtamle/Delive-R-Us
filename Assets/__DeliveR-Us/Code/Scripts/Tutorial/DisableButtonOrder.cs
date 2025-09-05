using UnityEngine;
using UnityEngine.UI;

public class DisableButtonOrder : MonoBehaviour
{
    public Button declineOrder;

    public void SetButtonInteractable(bool canInteract)
    {
        if (declineOrder != null)
        {
            declineOrder.interactable = canInteract;
        }
        else
        {
            Debug.LogWarning("[DisableButtonOrder] declineOrder button chưa được gán!");
        }
    }

    public void ToggleButton()
    {
        if (declineOrder != null)
        {
            declineOrder.interactable = !declineOrder.interactable;
        }
    }
}
