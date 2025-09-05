using UnityEngine;

public class ManualUnstuck : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode unstuckKey = KeyCode.R;
    public float checkRadius = 1f;
    public float pushDistance = 2f;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(unstuckKey))
    //    {
    //        Unstuck();
    //    }
    //}

    private void Unstuck()
    {
        Vector3[] directions = {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right,
            (transform.forward + transform.right).normalized,
            (transform.forward - transform.right).normalized,
            (-transform.forward + transform.right).normalized,
            (-transform.forward - transform.right).normalized
        };

        bool moved = false;

        foreach (var dir in directions)
        {
            if (!Physics.Raycast(transform.position, dir, checkRadius))
            {
                Vector3 targetPos = transform.position + dir * pushDistance;
                transform.position = targetPos;
                moved = true;
                break;
            }
        }

        if (!moved)
        {
            transform.position = transform.position + Vector3.up * pushDistance;
        }

        Debug.Log("Unstuck at: " + transform.position);
    }
}
