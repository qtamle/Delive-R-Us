using TMPro;
using UnityEngine;

public class MsgOverlayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _msgText;

    public void UpdateMsg(string msg)
    {
        _msgText.text = msg;
    }

}
