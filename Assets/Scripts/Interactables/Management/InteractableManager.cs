using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance { get; private set; }
    protected Dictionary<Transform, Interactable> interactables = new();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Interact(Transform _interactable, Transform _interactor)
    {
        Interactable _int;
        if (interactables.TryGetValue(_interactable, out _int))
        {
            _int.OnInteract(_interactor);
        }
    }
    public bool IsInteractable(Transform _interactable)
    {
        return interactables.ContainsKey(_interactable);
    }
    public void Register(Interactable _interactable)
    {
        if (!interactables.ContainsKey(_interactable.transform))
        {
            interactables.Add(_interactable.transform, _interactable);
        }
    }
    public void DeRegister(Transform _transform)
    {
        if (interactables.ContainsKey(_transform))
        {
            interactables.Remove(_transform);
        }
    }
}
