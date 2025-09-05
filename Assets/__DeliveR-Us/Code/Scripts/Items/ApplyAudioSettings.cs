using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class ApplyAudioSettings : MonoBehaviour
{
    [SerializeField] private bool _isSFX = true;

    private AudioSource _iSrc;
    private float _initialVol = 0.7f;

    private void OnEnable()
    {
        if (_iSrc == null)
        {
            _iSrc = GetComponent<AudioSource>();
            _initialVol = _iSrc.volume;

        }

        UpdateVolume();

        AudioManager.OnSettingsChanges += UpdateVolume;
    }

    private void OnDisable()
    {
        AudioManager.OnSettingsChanges -= UpdateVolume;
    }

    private void UpdateVolume()
    {
        if (_isSFX)
            _iSrc.volume = _initialVol * AudioManager.Instance.GetAudioSettings.SfxVolume;
        else
            _iSrc.volume = _initialVol * AudioManager.Instance.GetAudioSettings.BgVolume;
    }
}
