using UnityEngine;

public class HPComponent : MonoBehaviour
{
    public float maxhp { get; protected set; } = -1;
    public float currenthp { get; protected set; }
    public void SetMaxHP(float maxhp)
    {
        if (this.maxhp < 0)
        {
            this.maxhp = maxhp;
        }
    }
    private void OnEnable()
    {
        Reset();
    }
    public void Reset()
    {
        currenthp = maxhp;
    }
    public void TakeDamage(float damage)
    {
        currenthp -= damage;
        if (currenthp <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        transform.root.gameObject.SetActive(false);
    }
}
