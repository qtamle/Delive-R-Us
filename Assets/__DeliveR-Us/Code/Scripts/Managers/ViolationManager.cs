using UnityEngine;

public class ViolationManager : MonoBehaviour
{
    [Header("References")]
    public ViolationData violationData;

    private void OnEnable()
    {
        if (violationData != null)
        {
            violationData.OnViolationChanged += OnViolationChanged;
            violationData.LoadViolations(); 
        }
    }

    private void OnDisable()
    {
        if (violationData != null)
            violationData.OnViolationChanged -= OnViolationChanged;
    }

    private void OnViolationChanged(int current, int max)
    {
        if (violationData.IsGameOver())
        {
            Debug.Log("[ViolationManager] GAME OVER!");
            if (GameOver.Instance != null)
                GameOver.Instance.ShowGameOver("All that hustle... for nothing. I lost it all because of a few mistakes. This job was all I had.");
        }
    }
}
