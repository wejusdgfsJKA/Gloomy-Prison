using UnityEngine;
using UnityEngine.Events;

public class StaminaComponent : MonoBehaviour
{
    public int MaxStamina { get; protected set; } = -1;
    public int CurrentStamina { get; protected set; }
    [field: SerializeField]
    public UnityEvent OnStagger { get; protected set; }
    public void SetMaxStamina(int maxhp)
    {
        if (MaxStamina < 0)
        {
            MaxStamina = maxhp;
        }
    }
    public void Reset()
    {
        CurrentStamina = MaxStamina;
    }
    public bool DrainStamina(int _value)
    {
        CurrentStamina -= _value;
        if (CurrentStamina <= 0)
        {
            OnStagger?.Invoke();
            return false;
        }
        return true;
    }
}
