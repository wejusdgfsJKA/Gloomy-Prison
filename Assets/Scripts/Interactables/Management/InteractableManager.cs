using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    /// <summary>
    /// The instance of this manager, only one can be active at any given time
    /// </summary>
    public static InteractableManager Instance { get; private set; }
    protected Dictionary<Transform, Interactable> interactables = new();
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    /// <summary>
    /// An entity is trying to interact with an object.
    /// </summary>
    /// <param name="target">The object in question.</param>
    /// <param name="interactor">The entity in question.</param>
    /// <returns>True if the interaction was successfull, false otherwise.</returns>
    public bool Interact(Transform target, Transform interactor)
    {
        Interactable interactable;
        if (interactables.TryGetValue(target, out interactable))
        {
            interactable.Interact(interactor);
            return true;
        }
        return false;
    }
    public bool IsInteractable(Transform interactable)
    {
        return interactables.ContainsKey(interactable);
    }
    /// <summary>
    /// Register an interactable, so it can be interacted with.
    /// </summary>
    /// <param name="interactable">The interactable to be registered.</param>
    public void Register(Interactable interactable)
    {
        if (!interactables.ContainsKey(interactable.transform))
        {
            interactables.Add(interactable.transform, interactable);
        }
    }
    /// <summary>
    /// De-register an interactable, so it can no longer be interacted with.
    /// </summary>
    /// <param name="transform">The interactable to be de-registered.</param>
    /// <returns>True if de-registration was successfull, false otherwise.</returns>
    public bool DeRegister(Transform transform)
    {
        return interactables.Remove(transform);
    }
}
