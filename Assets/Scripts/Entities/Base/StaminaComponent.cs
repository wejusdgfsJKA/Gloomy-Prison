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
    public int RegenRate { get; set; }
    /// <summary>
    /// If reduced to zero stamina, how much time must pass before we begin 
    /// regenerating.
    /// </summary>
    public int NoRegenTime { get; set; }
    /// <summary>
    /// When were we reduced to zero stamina.
    /// </summary>
    protected float TimeReducedToZero = -1;
    protected void OnEnable()
    {
        Reset();
    }
    public void Reset()
    {
        CurrentStamina = MaxStamina;
        TimeReducedToZero = -1;
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
    public void FixedUpdate()
    {
        if (TimeReducedToZero == -1)
        {
            CurrentStamina += RegenRate;
            if (CurrentStamina > MaxStamina)
            {
                CurrentStamina = MaxStamina;
            }
        }
        else
        {
            if (Time.time - TimeReducedToZero >= NoRegenTime)
            {
                TimeReducedToZero = -1;
            }
        }
    }
}
