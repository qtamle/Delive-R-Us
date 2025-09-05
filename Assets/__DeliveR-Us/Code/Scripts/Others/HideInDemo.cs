using UnityEngine;

public class HideInDemo : MonoBehaviour
{
    void Start()
    {
        ApplyVisibility();
    }

    private void ApplyVisibility()
    {
        if (GameManager.Instance.demoVersion)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
