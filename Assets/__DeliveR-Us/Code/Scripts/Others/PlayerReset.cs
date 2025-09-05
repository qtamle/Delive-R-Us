using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    [Header("Reset Target")]
    public Transform resetPoint;

    private void OnEnable()
    {
        ResetEvents.OnResetPlayer += DoReset;
    }

    private void OnDisable()
    {
        ResetEvents.OnResetPlayer -= DoReset;
    }

    public void DoReset()
    {
        if (resetPoint == null) return;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = resetPoint.position;
        transform.rotation = resetPoint.rotation;

        if (cc != null) cc.enabled = true;
    }
}
