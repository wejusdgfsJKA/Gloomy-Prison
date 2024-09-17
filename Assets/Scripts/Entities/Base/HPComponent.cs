using UnityEngine;

public class HPComponent : MonoBehaviour
{
    public int MaxHP { get; protected set; } = -1;
    public int CurrentHP { get; protected set; }
    public void SetMaxHP(int _maxhp)
    {
        if (MaxHP < 0)
        {
            MaxHP = _maxhp;
        }
    }
    public void Reset()
    {
        CurrentHP = MaxHP;
    }
    public void TakeDamage(int _damage)
    {
        CurrentHP -= _damage;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }
    protected void Die()
    {
        transform.root.gameObject.SetActive(false);
    }
}
