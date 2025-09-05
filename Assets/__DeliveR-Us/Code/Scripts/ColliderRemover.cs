using UnityEngine;

using UnityEditor;
using Unity.VisualScripting;

public class ColliderRemover : MonoBehaviour
{

    [ContextMenu("RemoveColliders")]
    public void RemoveColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        int count = 0;

        foreach (Collider collider in colliders)
        {
            DestroyImmediate(collider);
            count++;
        }

        Debug.Log($"Removed {count} colliders from {gameObject.name} and its children.");
    }
}
