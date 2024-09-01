using UnityEngine;

public class HPComponent : MonoBehaviour
{
    public float MaxHP { get; protected set; } = -1;
    public float CurrentHP { get; protected set; }
    public void SetMaxHP(float maxhp)
    {
        if (MaxHP < 0)
        {
            MaxHP = maxhp;
        }
    }
    private void OnEnable()
    {
        Reset();
    }
    public void Reset()
    {
        CurrentHP = MaxHP;
    }
    public void TakeDamage(float damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        transform.root.gameObject.SetActive(false);
    }
}
