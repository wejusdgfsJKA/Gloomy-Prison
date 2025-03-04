using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AttackData")]
[System.Serializable]
public class Attack : ScriptableObject
{
    /// <summary>
    /// Light attacks can only stagger weaker enemies.
    /// </summary>
    public enum Weight { Light = 0, Normal = 1 }
    public enum Type { Strike = 0, Thrust = 1, Bash = 2 };
    //heavy attacks can only be successfully blocked with shields
    public enum Strength { Regular = 0, Heavy = 1 };
    [field: SerializeField]
    public int Damage { get; protected set; } = 0;
    [field: SerializeField]
    public Weight AtkWeight { get; protected set; } = Weight.Normal;
    [field: SerializeField]
    public int StaminaDamage { get; protected set; } = 0;
    [field: SerializeField]
    public Type AttackType { get; protected set; }
    [field: SerializeField]
    public Strength AttackStrength { get; protected set; } = Strength.Regular;
    [field: SerializeField]
    public bool Cancellable { get; protected set; } = true;
    [field: SerializeField]
    public bool Feintable { get; protected set; } = true;
    [field: SerializeField]
    public AnimationClip Regular { get; protected set; }
    [field: SerializeField]
    public AnimationClip Alternate { get; protected set; }
}
