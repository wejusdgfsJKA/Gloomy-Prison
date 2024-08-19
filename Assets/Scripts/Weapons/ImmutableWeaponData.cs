using UnityEngine;
//relevant for block angles, see the docs
public enum AttackType { Strike, Thrust, Push };
//heavy attacks can only be successfully blocked with shields
public enum AttackStrength { Regular, Heavy };
[CreateAssetMenu(menuName = "ScriptableObjects/WeaponData")]
[System.Serializable]
public class ImmutableWeaponData : ScriptableObject
{
    public string Name;
    public float CheckInterval = 0.01f;//how often should the hurtboxes check for
    //collisions; 0.01f seems to be reliable, anything slower will cause quick
    //animations to sometimes not register hits
    public int MaxHurtboxEntities = 10;
    //the enemy's layermask
    public LayerMask mask;
    //used for the AI to judge distances
    public float reach;
    public float StrikeBlockAngle, ThrustBlockAngle;
    public bool shield = false;
    public DmgInfo dmgInfo;
}
[System.Serializable]
public struct DmgInfo
{
    public float damage, staminaCost, poiseDamage;
    public AttackType attackType;
    public AttackStrength strength;
    //who dealt this damage
    public Transform owner;
    //relevant for block angles
    public Vector3 ContactPoint;
}
