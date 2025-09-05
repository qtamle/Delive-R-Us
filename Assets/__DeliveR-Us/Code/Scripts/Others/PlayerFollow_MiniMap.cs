using UnityEngine;

public class PlayerFollow_MiniMap : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    public void UpdatePlayerTransform(Transform playerTransform) => _playerTransform = playerTransform;

    private void LateUpdate()
    {
        transform.position = new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,_playerTransform.eulerAngles.y, transform.eulerAngles.z);
    }
}
