using UnityEngine;

public class ObjectInteractions : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Detection Parameters")]
    [SerializeField] private float _interactDistance = 5f;
    [SerializeField] private float _sphereRadius = 0.25f;
    [SerializeField] private LayerMask _interactableLayers;
    [SerializeField] private LayerMask _obstacleLayers;

    [Header("Interactions")]
    [SerializeField] private KeyCode _interaciontKey = KeyCode.F;

    private Interactable _currentInteractable;
    private Camera _mainCameraRef;

    #endregion

    #region Unity Methods

    private void Start()
    {
        _mainCameraRef = Camera.main;
    }

    private void Update()
    {
        DoObjectDetection();
    }

    #endregion

    private void DoObjectDetection()
    {
        Ray ray = _mainCameraRef.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.SphereCast(ray, _sphereRadius, out hit, _interactDistance, _interactableLayers))
        {
            if (Physics.Raycast(_mainCameraRef.transform.position, hit.point - _mainCameraRef.transform.position, Vector3.Distance(_mainCameraRef.transform.position, hit.point), _obstacleLayers))
            {
                ClearLastTarget();
            }
            else if (hit.collider != null && hit.transform.TryGetComponent(out Interactable detectedInteractable))
            {
                if (detectedInteractable != _currentInteractable && detectedInteractable.IsInteractive)
                {
                    ClearLastTarget();
                    _currentInteractable = detectedInteractable;
                    _currentInteractable.OnStartDetection();
                }
            }
        }
        else
        {
            ClearLastTarget();
        }

        if (_currentInteractable != null && Input.GetKeyDown(_interaciontKey))
        {
            _currentInteractable.Interact();
            _currentInteractable = null;
        }
    }
    public void ClearLastTarget()
    {
        if (_currentInteractable == null) return;

        _currentInteractable.OnStopDetection();
        _currentInteractable = null;
    }

    private void OnDrawGizmos()
    {
        if (_mainCameraRef == null)
            _mainCameraRef = Camera.main;

        if (_mainCameraRef == null) return;

        Vector3 origin = _mainCameraRef.transform.position;
        Vector3 direction = _mainCameraRef.transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + direction * _interactDistance);

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(origin + direction * _interactDistance, _sphereRadius);
    }


}
