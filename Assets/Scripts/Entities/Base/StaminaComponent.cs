using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Manages an entity's stamina.
/// </summary>
public class StaminaComponent : MonoBehaviour
{
    public int MaxStamina { get; set; }
    public int CurrentStamina { get; protected set; }
    /// <summary>
    /// Fires when this entity runs out of stamina.
    /// </summary>
    [field: SerializeField]
    public UnityEvent OnStagger { get; protected set; }
    public void Reset()
    {
        CurrentStamina = MaxStamina;
    }
    public bool DrainStamina(int value)
    {
        CurrentStamina -= value;
        if (CurrentStamina <= 0)
        {
            OnStagger?.Invoke();
            return false;
        }
        return true;
    }

}
