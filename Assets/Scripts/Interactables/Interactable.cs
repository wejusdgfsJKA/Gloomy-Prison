using UnityEngine;

public class Interactable : MonoBehaviour
{
    public System.Action<Transform> OnInteract { get; protected set; }
    protected void OnEnable()
    {
        InteractableManager.Instance.Register(this);
    }
    protected void OnDisable()
    {
        InteractableManager.Instance.DeRegister(transform);
    }
}
