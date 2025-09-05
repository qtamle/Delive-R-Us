using UnityEngine;

public abstract class Interactable: MonoBehaviour
{
    [SerializeField] protected Color _iTagColor = Color.yellow;
    [SerializeField] protected string _interactionMsg = "Press F to Interact With";

    public virtual bool IsInteractive { protected set; get; } = true;

    public virtual void Interact()
    {
        GameManager.Instance.HideInteractionUI();
        GameManager.Instance.OnInteractionExit();
    }
    public virtual void OnStartDetection(string itemTag = null)
    {
        string tag = $"<color=#{ColorUtility.ToHtmlStringRGB(_iTagColor)}>{itemTag}</color>";

        GameManager.Instance.DispalyInteractionUI($"{_interactionMsg} {tag}");
        GameManager.Instance.OnInteractionStart(Color.green);

    }
    public virtual void OnStopDetection()
    {
        GameManager.Instance.HideInteractionUI();
        GameManager.Instance.OnInteractionExit();
    }
}