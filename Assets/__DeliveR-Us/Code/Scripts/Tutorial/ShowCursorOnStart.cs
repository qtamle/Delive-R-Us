using UnityEngine;

public class ShowCursorOnStart : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToDisable;
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (objectsToDisable != null)
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}
