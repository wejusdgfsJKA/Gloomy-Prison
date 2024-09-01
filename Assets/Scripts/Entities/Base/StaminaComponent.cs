using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
    public float MaxStamina { get; protected set; } = -1;
    public float CurrentStamina { get; protected set; }
    public void SetMaxStamina(float maxhp)
    {
        if (MaxStamina < 0)
        {
            MaxStamina = maxhp;
        }
    }
    protected void OnEnable()
    {
        Reset();
    }
    public void Reset()
    {
        CurrentStamina = MaxStamina;
    }
    public void DrainStamina(float damage)
    {
        CurrentStamina -= damage;
        if (CurrentStamina <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        transform.root.gameObject.SetActive(false);
    }
}
