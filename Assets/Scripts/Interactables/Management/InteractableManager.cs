using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance { get; private set; }
    protected Dictionary<Transform, Interactable> interactables = new();
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Interact(Transform interactable, Transform interactor)
    {
        Interactable inter;
        if (interactables.TryGetValue(interactable, out inter))
        {
            inter.Interact(interactor);
        }
    }
    public bool IsInteractable(Transform interactable)
    {
        return interactables.ContainsKey(interactable);
    }
    public void Register(Interactable interactable)
    {
        if (!interactables.ContainsKey(interactable.transform))
        {
            interactables.Add(interactable.transform, interactable);
        }
    }
    public void DeRegister(Transform transform)
    {
        if (interactables.ContainsKey(transform))
        {
            interactables.Remove(transform);
        }
    }
}
