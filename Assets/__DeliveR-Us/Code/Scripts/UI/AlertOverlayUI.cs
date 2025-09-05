using TMPro;
using UnityEngine;

public class AlertOverlayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _alertHeaderText;
    [SerializeField] private TextMeshProUGUI _alertDescriptionText;

    public void UpdateMsg(string header ,string description)
    {
        _alertHeaderText.text = header;
        _alertDescriptionText.text = description;
    }
}
