using UnityEngine;

public class MiniMapIcon : MonoBehaviour
{
    [SerializeField] private float _maxDistanceFromCamera = 100f;

    [Header("Read-Only")]
    [SerializeField] private Transform _miniMapCamera;
    [SerializeField] private float _currentDistance = 0f;

    private Vector3 _originalPosition;
    private bool _isClamped = false;

    private void Start()
    {
        _miniMapCamera = FindFirstObjectByType<PlayerFollow_MiniMap>().transform;

        _originalPosition = transform.position;
    }

    private void Update()
    {
        Vector3 direction = _originalPosition - _miniMapCamera.position;
        direction.y = 0f;

        _currentDistance = direction.magnitude;

        if (_currentDistance > _maxDistanceFromCamera)
        {
            if (!_isClamped)
            {
                _isClamped = true;
            }

            direction.Normalize();
            Vector3 clampedPos = _miniMapCamera.position + direction * _maxDistanceFromCamera;

            transform.position = new Vector3(clampedPos.x, transform.position.y, clampedPos.z);
        }
        else
        {
            if (_isClamped)
            {
                transform.position = _originalPosition;
                _isClamped = false;
            }
        }

        Debug.DrawLine(_miniMapCamera.position, transform.position, Color.green);
    }
}
