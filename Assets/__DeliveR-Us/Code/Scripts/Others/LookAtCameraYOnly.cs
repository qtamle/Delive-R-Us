using UnityEngine;

public class LookAtCameraYOnly : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Start()
    {
        if (Camera.main != null)
            _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null) return;

        Vector3 targetPosition = _cameraTransform.position;

        // Keep only the Y rotation by aligning to camera horizontally
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(-direction);
    }
}
