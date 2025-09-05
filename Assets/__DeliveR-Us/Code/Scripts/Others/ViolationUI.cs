using TMPro;
using UnityEngine;

public class ViolationUI : MonoBehaviour
{
    [Header("References")]
    public ViolationData violationData;
    public TextMeshProUGUI violationText;

    private void OnEnable()
    {
        if (violationData != null)
        {
            violationData.OnViolationChanged += UpdateUI;
            violationData.LoadViolations(); 
        }
    }

    private void OnDisable()
    {
        if (violationData != null)
            violationData.OnViolationChanged -= UpdateUI;
    }

    private void UpdateUI(int current, int max)
    {
        if (violationText != null)
            violationText.text = $"{current}/{max}";
    }
}
