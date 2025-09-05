using System.Collections;
using UnityEngine;

public class GasTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        var gas = GasSystem.Instance;
        if (hasTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            gas.GasStation(this);
        }
    }

    public void SetTriggerEnabled(bool enable)
    {
        hasTriggered = false;
    }
}
