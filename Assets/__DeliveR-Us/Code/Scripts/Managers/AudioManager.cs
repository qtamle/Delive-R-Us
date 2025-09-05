using UnityEngine;
using MHUtility;
using System;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    #region Setters/Private Variables

    [Header("Resources")]
    [SerializeField] private AudiosData[] _audios;

    [Header("Controls")]
    [SerializeField] private AudioSettings _audioSettings;

    private AudioSource _sfxSrc;

    public static Action OnSettingsChanges = delegate { };


    #endregion

    #region Getters/Public Variables
    
    public AudioSettings GetAudioSettings => _audioSettings;

    #endregion

    public void PlaySFX(AudioId id)
    {
        if (GetAudioSettings.SfxVolume < 0.1f) return;

        AudiosData audioData = Array.Find(_audios, x=> x.Id == id);
        
        if(audioData == null)
        {
            Debug.Log($"{id}'s Data dont exist in AudioManager, Please Add!");
            return;
        }

        if (_sfxSrc == null)
        {
            _sfxSrc = gameObject.AddComponent<AudioSource>();

            _sfxSrc.playOnAwake = false;
            _sfxSrc.loop = false;
        }

        _sfxSrc.clip = audioData.Data.Clip;
        _sfxSrc.volume = audioData.Data.ClipVolume * GetAudioSettings.SfxVolume;
        _sfxSrc.outputAudioMixerGroup = audioData.AudioMixer.outputAudioMixerGroup;
        _sfxSrc.Play();
    }

    public void UpdateSettings(AudioSettings audioSettings)
    {
        _audioSettings = audioSettings;

        OnSettingsChanges?.Invoke();
    }

    public void FetchSettingsFromJson()
    {
        UpdateSettings(GameManager.Instance.GetPlayerDataSo.GetAudioSettings);

        OnSettingsChanges?.Invoke();
    }
    public void SaveSettings()
    {
        GameManager.Instance.GetPlayerDataSo.UpdateAudioSettings(_audioSettings);
    }
}


[System.Serializable]
public class AudioSettings
{
    [Range(0, 1)] public float BgVolume = 1;
    [Range(0, 1)] public float SfxVolume = 1;
}

[System.Serializable]
public class Audio
{
    public AudioClip Clip;
    public float ClipVolume = 0.75f;
}

[System.Serializable]
public class AudiosData
{
    public AudioId Id;
    public AudioMixer AudioMixer;
    public Audio Data;
}

public enum AudioId
{
    InteractionStart = 0,
    RoomDoor = 1,
    Teleport = 2,
    Msg = 3,
    BtnClick = 4,
    CollectItem = 5,
    Alert = 6,
    Success =7,
    Eat =8,
    PayMoney = 9,
    BorrowMoney = 10,
    AlarmClock = 11,
    Rooster = 12,
    Bill = 13,
    Complete = 14,
    GameOver = 15,
}