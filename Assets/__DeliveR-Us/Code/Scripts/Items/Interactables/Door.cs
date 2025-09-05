using UnityEngine;

public class Door : Interactable
{
    #region Setters/Private Variables

    [SerializeField] private string _iTag = "Door";

    [Header("Parameters")]
    [SerializeField] private float _openRotY = 110f;
    [SerializeField] private float _closeRotY = 0f;
    [SerializeField] private float _rotateTime = 3f;

    [Header("Resources")]
    [SerializeField] private GameObject _doorObj;

    #endregion

    #region Getters/Public Variables

    public bool IsOpened { private set; get; } = false;

    #endregion

    public override void Interact()
    {
        if (!IsInteractive) return;

        base.Interact();

        AudioManager.Instance.PlaySFX(AudioId.RoomDoor);

        IsInteractive = false;

        IsOpened = !IsOpened;

        float targetRot = IsOpened ? _openRotY : _closeRotY;

        LeanTween.cancel(_doorObj);
        _doorObj.LeanRotateY(targetRot, _rotateTime).setOnComplete(() => 
        {
            IsInteractive = true;
        });
    }
    public override void OnStartDetection(string itemTag =null)
    {
        if (!IsInteractive) return;

        base.OnStartDetection(_iTag);
    }
    public override void OnStopDetection()
    {
        base.OnStopDetection();

    }

}
