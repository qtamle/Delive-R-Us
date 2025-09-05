using UnityEngine;

public class ScooterReset : MonoBehaviour
{
    [Header("Reset Target")]
    public Transform resetPoint;

    private void OnEnable()
    {
        ResetEvents.OnResetScooter += DoReset;
    }

    private void OnDisable()
    {
        ResetEvents.OnResetScooter -= DoReset;
    }

    public void DoReset()
    {
        if (resetPoint == null) return;

        transform.position = resetPoint.position;
        transform.rotation = resetPoint.rotation;
    }
}
