using UnityEngine;

public class CloneReset : MonoBehaviour
{
    [Header("Scooter Ref")]
    public Transform scooter;

    private void Start()
    {
        if (scooter == null)
        {
            var rc = FindFirstObjectByType<ResourceController>();
            if (rc != null) scooter = rc.position;
        }
    }

    private void OnEnable()
    {
        ResetEvents.OnResetClone += DoReset;
    }

    private void OnDisable()
    {
        ResetEvents.OnResetClone -= DoReset;
    }

    public void DoReset()
    {
        if (scooter == null) return;

        transform.position = scooter.position;
        transform.rotation = scooter.rotation;
    }
}
