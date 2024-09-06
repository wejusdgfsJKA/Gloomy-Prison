using Animancer;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/AttackData")]
[System.Serializable]
public class Attack : ScriptableObject
{
    public enum Form { Regular = 0, Combo = 1, Feint = 2, Counter = 3 }
    public enum Type { Light = 0, Thrust = 1, Overhead = 2, Jab = 3, Kick = 4 };
    //heavy attacks can only be successfully blocked with shields
    public enum Strength { Regular = 0, Heavy = 1 };
    [field: SerializeField]
    public float Damage { get; protected set; }
    [field: SerializeField]
    public float StaminaDrain { get; protected set; }
    [field: SerializeField]
    public Type AttackType { get; protected set; }
    [field: SerializeField]
    public Strength AttackStrength { get; protected set; } = Strength.Regular;
    [field: SerializeField]
    public AnimationClip Clip { get; protected set; }
    [field: SerializeField]
    public ClipTransition Riposte { get; protected set; }
    [field: SerializeField]
    public ClipTransition Counter { get; protected set; }
    public AnimationClip AltClip { get; protected set; }
    [field: SerializeField]
    public ClipTransition AltRiposte { get; protected set; }
    [field: SerializeField]
    public ClipTransition AltCounter { get; protected set; }
    [field: SerializeField]
    public bool Cancelable { get; protected set; } = true;
    [field: SerializeField]
    public bool Feintable { get; protected set; } = true;
}
