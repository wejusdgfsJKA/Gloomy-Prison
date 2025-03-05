using UnityEngine;

public abstract class HPComponent : MonoBehaviour
{
    public int MaxHP { get; set; }
    /// <summary>
    /// How much HP the entity has right now.
    /// </summary>
    [field: SerializeField]
    public int CurrentHP { get; protected set; }
    public System.Action<int> TakeDamage { get; protected set; }
    protected void OnEnable()
    {
        Reset();
    }
    /// <summary>
    /// Reset the CurrentHP variable.
    /// </summary>
    protected void Reset()
    {
        CurrentHP = MaxHP;
    }
}
