namespace MHUtility
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using UnityEngine;

    public static class SaveManager
    {
        private static readonly string _savePath = Path.Combine(Application.persistentDataPath, "PD_DU.dat");
        private static readonly string _path = Path.Combine(Application.persistentDataPath, "TD.dat");
        private static readonly string _encryptionKey = "DeliveR-Us_12345";

        public static void LoadData(ref PlayerDataSO playerDataSO)
        {
            if (playerDataSO == null)
            {
                Debug.LogError("[SaveManager] PlayerDataSO reference is null!");
                return;
            }

            PlayerData data = new PlayerData();
            data.ResetData();

            if (File.Exists(_savePath))
            {
                try
                {
                    string encrypted = File.ReadAllText(_savePath);
                    string json = Decrypt(encrypted, _encryptionKey);
                    data = JsonUtility.FromJson<PlayerData>(json);
                }
                catch (Exception e)
                {
                    Debug.Log($"[SaveManager] Primary save failed: {e.Message}");

                    string backupPath = _savePath + ".bak";
                    if (File.Exists(backupPath))
                    {
                        try
                        {
                            string backupEncrypted = File.ReadAllText(backupPath);
                            string backupJson = Decrypt(backupEncrypted, _encryptionKey);
                            data = JsonUtility.FromJson<PlayerData>(backupJson);
                            Debug.Log("[SaveManager] Successfully recovered from backup.");
                        }
                        catch (Exception backupEx)
                        {
                            Debug.Log($"[SaveManager] Failed to load backup: {backupEx.Message}");
                        }
                    }
                }
            }

            playerDataSO.UpdateData(data);
        }
        public static void SaveData(PlayerDataSO playerDataSO)
        {
            if (playerDataSO == null)
            {
                Debug.LogError("[SaveManager] PlayerDataSO reference is null!");
                return;
            }

            playerDataSO.UpdateSaveVersion(IncrementVersion(playerDataSO.GetSaveVersion));

            string json = JsonUtility.ToJson(playerDataSO.GetPlayerData);

            string encrypted = Encrypt(json, _encryptionKey);

            if (File.Exists(_savePath))
            {
                File.Copy(_savePath, _savePath + ".bak", overwrite: true);
            }

            File.WriteAllText(_savePath, encrypted);

            Debug.Log($"[SaveManager] Save written to: {_savePath}");
        }

        public static void DeleteSave()
        {
            if (File.Exists(_savePath)) File.Delete(_savePath);
            if (File.Exists(_savePath + ".bak")) File.Delete(_savePath + ".bak");
            Debug.Log("[SaveManager] Save and backup deleted.");
        }

        public static bool SaveExists()
        {
            return File.Exists(_savePath);
        }

        public static void SaveDayProgress(float elapsedTime, int currentDay)
        {
            DayProgressData data = new DayProgressData
            {
                ElapsedTime = elapsedTime,
                CurrentDay = currentDay
            };

            string json = JsonUtility.ToJson(data);
            string encrypted = Encrypt(json, _encryptionKey);

            File.WriteAllText(_path, encrypted);
        }

        public static DayProgressData LoadDayProgress()
        {
            if (!File.Exists(_path))
            {
                Debug.LogWarning("[SaveManager] No DayProgress file found, using default.");
                return new DayProgressData { ElapsedTime = 0f, CurrentDay = 1 };
            }

            try
            {
                string encrypted = File.ReadAllText(_path);
                string decrypted = Decrypt(encrypted, _encryptionKey);
                return JsonUtility.FromJson<DayProgressData>(decrypted);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to load DayProgress: {e}");
                return new DayProgressData { ElapsedTime = 0f, CurrentDay = 1 };
            }
        }

        public static void DeleteDayProgress()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
                Debug.Log("[SaveManager] DayProgress file deleted.");
            }
            else
            {
                Debug.LogWarning("[SaveManager] No DayProgress file to delete.");
            }
        }

        private static string IncrementVersion(string version)
        {
            string[] parts = version.Split('.');
            int[] numbers = new int[4];

            for (int i = 0; i < 4; i++)
            {
                if (i < parts.Length && int.TryParse(parts[i], out int num))
                    numbers[i] = num;
                else
                    numbers[i] = 0;
            }

            // Increment build and handle rollovers
            numbers[3]++; // build
            if (numbers[3] > 99)
            {
                numbers[3] = 0;
                numbers[2]++; // patch

                if (numbers[2] > 99)
                {
                    numbers[2] = 0;
                    numbers[1]++; // minor

                    if (numbers[1] > 99)
                    {
                        numbers[1] = 0;
                        numbers[0]++; // major
                    }
                }
            }

            return $"{numbers[0]}.{numbers[1]}.{numbers[2]}.{numbers[3]}";
        }

        private static string Encrypt(string plainText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] combined = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, combined, iv.Length, encrypted.Length);

            return Convert.ToBase64String(combined);
        }

        private static string Decrypt(string cipherText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            byte[] combined = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;

            byte[] iv = new byte[16];
            Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
            aes.IV = iv;

            byte[] encrypted = new byte[combined.Length - iv.Length];
            Buffer.BlockCopy(combined, iv.Length, encrypted, 0, encrypted.Length);

            using ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}