using UnityEngine;
//relevant for block angles, see the docs
public enum AttackType { Strike, Thrust, Kick };
//heavy attacks can only be successfully blocked with shields
public enum AttackStrength { Regular, Heavy };
[CreateAssetMenu(menuName = "ScriptableObjects/WeaponData")]
[System.Serializable]
public class WeaponData : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; }
    [field: SerializeField]
    public float CheckInterval { get; } = 0.01f;//how often should the hurtboxes check for
    //collisions; 0.01f seems to be reliable, anything slower will cause quick
    //animations to sometimes not register hits
    [field: SerializeField]
    public int MaxHurtboxEntities { get; } = 10;
    //the enemy's layermask
    [field: SerializeField]
    public LayerMask Mask { get; }
    //used for the AI to judge distances
    [field: SerializeField]
    public float Reach { get; }
    [field: SerializeField]
    public float StrikeBlockAngle { get; }
    [field: SerializeField]
    public float ThrustBlockAngle { get; }
    [field: SerializeField]
    public bool Shield { get; } = false;
    //for a basic moveset:
    //element 0 will be swing data,
    //element 1 will be thrust data,
    //element 2 will be push or shield bash data
    [field: SerializeField]
    public Attack[] Attacks { get; }
}