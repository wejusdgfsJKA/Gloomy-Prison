using UnityEngine;

public class HPComponent : MonoBehaviour
{
    public int MaxHP { get; set; }
    [field: SerializeField]
    public int CurrentHP { get; protected set; }
    public System.Action<int> TakeDamage { get; protected set; }
    protected void OnEnable()
    {
        Reset();
    }
    protected void Reset()
    {
        CurrentHP = MaxHP;
    }
}
