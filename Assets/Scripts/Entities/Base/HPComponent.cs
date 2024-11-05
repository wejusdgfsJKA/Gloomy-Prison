using UnityEngine;

public class HPComponent : MonoBehaviour
{
    public int MaxHP { get; protected set; } = -1;
    [field: SerializeField]
    public int CurrentHP { get; protected set; }
    public System.Action<int> TakeDamage { get; protected set; }
    public void SetMaxHP(int _maxhp)
    {
        if (MaxHP < 0)
        {
            MaxHP = _maxhp;
        }
    }
    protected void OnEnable()
    {
        Reset();
    }
    protected void Reset()
    {
        CurrentHP = MaxHP;
    }
}
