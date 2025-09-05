using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void QuitGame()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
