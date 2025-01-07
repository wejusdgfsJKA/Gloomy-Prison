using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected System.Action<Transform> OnInteract;
    protected void OnEnable()
    {
        InteractableManager.Instance.Register(this);
    }
    protected void OnDisable()
    {
        InteractableManager.Instance.DeRegister(transform);
    }
    public void AddAction(System.Action<Transform> action)
    {
        OnInteract += action;
    }
    public void Interact(Transform source)
    {
        OnInteract(source);
    }
}
