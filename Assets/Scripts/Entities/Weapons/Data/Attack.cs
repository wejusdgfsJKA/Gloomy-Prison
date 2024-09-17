using Animancer;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/AttackData")]
[System.Serializable]
public class Attack : ScriptableObject
{
    public enum Type { Light = 0, Thrust = 1, Overhead = 2, Jab = 3, Kick = 4 };
    //heavy attacks can only be successfully blocked with shields
    public enum Strength { Regular = 0, Heavy = 1 };
    [field: SerializeField]
    public int Damage { get; protected set; } = 0;
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
    public ClipTransition Regular { get; protected set; }
    [field: SerializeField]
    public ClipTransition Alternate { get; protected set; }
}
