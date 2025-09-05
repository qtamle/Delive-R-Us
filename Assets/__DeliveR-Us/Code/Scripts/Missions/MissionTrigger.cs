using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MissionTrigger : MonoBehaviour
{
    public MissionNpcRole role = MissionNpcRole.Delivery; // được gán khi spawn
    private bool hasTriggered = false;
    private Collider _col;

    public MissionStage CurrentStage { get; set; } = MissionStage.None;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        if (_col)
            _col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;
        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;
        MissionManager.Instance?.GetMissionTrigger(this);
    }

    // Đúng nghĩa enable/disable trigger, KHÔNG gọi DestroyPrefab ở đây nữa
    public void SetTriggerEnabled(bool enable)
    {
        if (_col)
            _col.enabled = enable;
        hasTriggered = !enable ? true : false; // bật lại thì reset; tắt thì giữ đã-trigger
    }
}

public enum MissionNpcRole
{
    Delivery,
    Package,
}

public enum MissionStage
{
    None,
    Accepted,
    AtPackage,
    Completed,
}
