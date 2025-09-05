using System;
using TMPro;
using UnityEngine;

public class ScooterRadioUI : MonoBehaviour
{
    [SerializeField] private GameObject _contentHolderUI;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _artistText;
    [SerializeField] private TextMeshProUGUI _albumText;

    public static Action<RadioAudio> ToggleUIAction = delegate { };

    private void OnEnable()
    {
        ToggleUIAction += ToggleUI;
    }

    private void OnDisable()
    {
        ToggleUIAction -= ToggleUI;
    }

    private void ToggleUI(RadioAudio newAudio)
    {
        _contentHolderUI.SetActive(false);

        if (newAudio == null) return;

        _titleText.text = newAudio.Title;
        _artistText.text = $"Artist: {newAudio.ArtistName}";
        _albumText.text = $"Album: {newAudio.Album}";

        _contentHolderUI.SetActive(true);
    }
}
