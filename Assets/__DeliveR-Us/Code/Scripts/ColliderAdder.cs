using UnityEngine;

public class ColliderAdder : MonoBehaviour
{
    public enum ColliderType
    {
        Box,
        Sphere,
        Capsule,
        Mesh
    }

    [Header("Collider Settings")]
    public ColliderType colliderType = ColliderType.Box;

    [ContextMenu("Add Colliders")]
    public void AddColliders()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        int count = 0;

        foreach (Transform t in transforms)
        {
            if (t.TryGetComponent<MeshRenderer>(out MeshRenderer renderer) && renderer.enabled && renderer.gameObject.activeInHierarchy)
            {
                if (!t.GetComponent<Collider>())
                {
                    AddCollider(t.gameObject);
                    count++;
                }
            }
        }

        Debug.Log($"Added {count} colliders to {gameObject.name} and its children.");
    }

    private void AddCollider(GameObject obj)
    {
        switch (colliderType)
        {
            case ColliderType.Box:
                obj.AddComponent<BoxCollider>();
                break;
            case ColliderType.Sphere:
                obj.AddComponent<SphereCollider>();
                break;
            case ColliderType.Capsule:
                obj.AddComponent<CapsuleCollider>();
                break;
            case ColliderType.Mesh:
                MeshCollider meshCol = obj.AddComponent<MeshCollider>();
                break;
        }
    }
}
