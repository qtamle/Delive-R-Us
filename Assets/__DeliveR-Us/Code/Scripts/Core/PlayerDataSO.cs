using MHUtility;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField] PlayerData PlayerData;

    public PlayerData GetPlayerData => PlayerData;

    public string GetPlayerName => PlayerData.PlayerName;
    public string GetSaveVersion => PlayerData.SaveVersion;
    public string GetLastPayoutDate => PlayerData.LastPayoutDate;

    public float GetWalletCoins => PlayerData.WalletCoins;
    public float GetTotalBalanceCoins => PlayerData.PlayerCoins;
    public long GetLastPayoutTimestamp => PlayerData.LastPayoutTimestamp;

    public AudioSettings GetAudioSettings => PlayerData.AudioSettings;

    public List<string> GetBoughtItems => PlayerData.BoughtItems;

    public void UpdateData(PlayerData loadedData)
    {
        PlayerData = loadedData;
    }

    public void UpdatePlayerName(string newName, bool saveData = true)
    {
        PlayerData.PlayerName = newName;

        if (saveData)
            GameManager.Instance.SaveData();
    }
    public void UpdateSaveVersion(string newSaveVersion)
    {
        PlayerData.SaveVersion = newSaveVersion;
    }
    public void UpdateLastPayout(string newLastPayout, bool saveData = true)
    {
        PlayerData.LastPayoutDate = newLastPayout;

        if (saveData)
            GameManager.Instance.SaveData();
    }

    public void AddCoinsInWallet(float amt, bool saveData = true)
    {
        PlayerData.WalletCoins += amt;

        if (saveData)
            GameManager.Instance.SaveData();
    }
    public void RemovePlayerCoins(float amt, bool saveData = true)
    {
        PlayerData.PlayerCoins -= amt;

        if (saveData)
            GameManager.Instance.SaveData();
    }

    public bool CanBuyItem(float itemPrice)
    {
        return itemPrice < PlayerData.PlayerCoins;
    }
    public void OnItemBought(string itemTitle, bool saveData= true)
    {
        PlayerData.BoughtItems.Add(itemTitle);

        if (saveData)
            GameManager.Instance.SaveData();
    }

    public void UpdateAudioSettings(AudioSettings audioSettings, bool saveData = true)
    {
        PlayerData.AudioSettings = audioSettings;

        PlayerPrefs.SetFloat("MusicVolume", audioSettings.BgVolume);
        PlayerPrefs.SetFloat("SfxVolume", audioSettings.SfxVolume);
        PlayerPrefs.Save();

        if (saveData)
            GameManager.Instance.SaveData();
    }
    public void DoPayout(bool saveData = true)
    {
        PlayerData.PlayerCoins += PlayerData.WalletCoins;
        PlayerData.WalletCoins = 0;

        if (saveData)
            GameManager.Instance.SaveData();
    }

    public void UpdateLastPayoutTimestamp(long timestamp, bool saveData = true)
    {
        PlayerData.LastPayoutTimestamp = timestamp;

        if (saveData)
            GameManager.Instance.SaveData();
    }

    [ContextMenu("Reset Data")]

    private void Reset()
    {
        PlayerData.ResetData();
        SaveManager.SaveData(this);
    }
}


[Serializable]
public class PlayerData
{
    public string PlayerName;
    public float PlayerCoins;
    public float WalletCoins;
    public string LastPayoutDate;

    public long LastPayoutTimestamp;

    [Header("Version")]
    public string SaveVersion;

    [Header("Audio")]
    public AudioSettings AudioSettings;

    [Header("Inventory")]
    public List<string> BoughtItems = new List<string>();

    public void ResetData()
    {
        PlayerName = "Max Allen";
        PlayerCoins = 100;
        WalletCoins = 0;
        LastPayoutDate = "";
        LastPayoutTimestamp = 0;
        SaveVersion = "0.0.0.1";

        float bgVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 1f;
        float sfxVolume = PlayerPrefs.HasKey("SfxVolume") ? PlayerPrefs.GetFloat("SfxVolume") : 1f;

        AudioSettings = new AudioSettings
        {
            BgVolume = bgVolume,
            SfxVolume = sfxVolume
        };
    }

}

[System.Serializable]
public class DayProgressData
{
    public float ElapsedTime;
    public int CurrentDay;
}

