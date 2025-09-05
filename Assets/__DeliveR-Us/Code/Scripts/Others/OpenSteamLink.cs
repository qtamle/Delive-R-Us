using UnityEngine;

public class OpenSteamLink : MonoBehaviour
{
    [SerializeField] private string steamUrl = "https://store.steampowered.com/app/3908120/Delive_R_us/";

    public void OpenSteamPage()
    {
        if (!string.IsNullOrEmpty(steamUrl))
        {
            Application.OpenURL(steamUrl);
        }
    }
}
