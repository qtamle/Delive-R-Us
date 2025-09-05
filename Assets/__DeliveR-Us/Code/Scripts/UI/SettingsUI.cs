using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;


    private void Start()
    {
        _soundVolumeSlider.onValueChanged.RemoveAllListeners();
        _musicVolumeSlider.onValueChanged.RemoveAllListeners();

        _soundVolumeSlider.onValueChanged.AddListener(soundVolumeSlider_fn);
        _musicVolumeSlider.onValueChanged.AddListener(musicVolumeSlider_fn);
    }

    private void OnEnable()
    {
        _soundVolumeSlider.value = AudioManager.Instance.GetAudioSettings.SfxVolume;
        _musicVolumeSlider.value = AudioManager.Instance.GetAudioSettings.BgVolume;
    }

    private void OnDisable()
    {
        AudioManager.Instance.SaveSettings();
    }

    private void soundVolumeSlider_fn(float vol)
    {
        AudioManager.Instance.UpdateSettings(new AudioSettings { BgVolume = _musicVolumeSlider.value, SfxVolume = _soundVolumeSlider.value });
    }

    private void musicVolumeSlider_fn(float vol)
    {
        AudioManager.Instance.UpdateSettings(new AudioSettings { BgVolume = _musicVolumeSlider.value, SfxVolume = _soundVolumeSlider.value });
    }
}
