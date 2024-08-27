using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/AttackData")]
[System.Serializable]
public class Attack : ScriptableObject
{
    public enum Type { Light, Overhead, Jab, Thrust, Kick };
    //heavy attacks can only be successfully blocked with shields
    public enum Strength { Regular, Heavy };
    [field: SerializeField]
    public float Damage { get; }
    [field: SerializeField]
    public float StaminaDrain { get; }
    [field: SerializeField]
    public Type AttackType { get; }
    [field: SerializeField]
    public Strength AttackStrength { get; } = Strength.Regular;
    [field: SerializeField]
    public bool Alternate { get; } = false;
    [field: SerializeField]
    public AnimationClip clip { get; }
}
