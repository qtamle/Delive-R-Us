using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class ScooterRadio : MonoBehaviour
{
    [SerializeField] private RadioAudio[] _radioMix;
    [SerializeField] private KeyCode _togglekey = KeyCode.Tab;

    private AudioSource _iSrc;
    private float _initialVol = 0.7f;
    private int _currentMusicIndex = 0;


    private void OnEnable()
    {
        if (_iSrc == null)
        {
            _iSrc = GetComponent<AudioSource>();
            _initialVol = _iSrc.volume;

        }

        AudioManager.OnSettingsChanges += UpdateVolume;

        if (_radioMix.Length > 0)
        {
            _currentMusicIndex = Random.Range(0, _radioMix.Length);
            PlayMusic(_currentMusicIndex);
        }
    }
    private void OnDisable()
    {
        AudioManager.OnSettingsChanges -= UpdateVolume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_togglekey))
        {
            ChangeMusic();
        }
    }
    private void PlayMusic(int index = 0)
    {
        if (_radioMix.Length == 0) return;

        RadioAudio radioAudio = _radioMix[index];
        ScooterRadioUI.ToggleUIAction?.Invoke(radioAudio);

        _iSrc.clip = radioAudio.AudioClip;
        _initialVol = radioAudio.Volume;

        double startTime = UnityEngine.AudioSettings.dspTime + 0.05;
        _iSrc.PlayScheduled(startTime);

        UpdateVolume();

        double length = _iSrc.clip.length;
        LeanTween.cancel(gameObject);
        LeanTween.delayedCall(gameObject, (float)length, ChangeMusic);
    }
    private void ChangeMusic()
    {
        if (_radioMix.Length == 0) return;

        _currentMusicIndex = (_currentMusicIndex + 1) % _radioMix.Length;
        PlayMusic(_currentMusicIndex);
    }

    private void UpdateVolume()
    {
        float newVol = AudioManager.Instance.GetAudioSettings.BgVolume;

        if (newVol < 0.05f)
        {
            _iSrc.Stop();
            ScooterRadioUI.ToggleUIAction?.Invoke(null);
        }
        else
        {
            _iSrc.volume = _initialVol * newVol;
            
            if(!_iSrc.isPlaying)
                _iSrc.Play();

        }
    }
}

[System.Serializable]
public class RadioAudio
{
    public AudioClip AudioClip;
    public float Volume = 0.6f;

    [Space]
    public string Title;
    public string ArtistName;
    public string Album;
}