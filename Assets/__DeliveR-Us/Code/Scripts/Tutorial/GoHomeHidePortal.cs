using UnityEngine;

public class GoHomeHidePortal : MonoBehaviour
{
    [SerializeField] private GameObject areaBlue;
    void Start()
    {
        if (PlayerPrefs.GetInt("HidePortal") == 1)
        {
            areaBlue.SetActive(false);
        }
    }

}
