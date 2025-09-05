using System;
using UnityEngine;

public class ResetManager : MonoBehaviour
{
    public static ResetManager Instance;

    public event Action OnReset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TriggerReset(bool isOnScooter)
    {
        if (isOnScooter)
        {
            ResetEvents.OnResetClone?.Invoke();
        }
        else
        {
            ResetEvents.OnResetPlayer?.Invoke();
            ResetEvents.OnResetScooter?.Invoke();
        }
    }
}
