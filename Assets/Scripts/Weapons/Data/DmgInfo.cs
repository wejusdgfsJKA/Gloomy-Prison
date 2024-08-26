using UnityEngine;

[System.Serializable]
public class DmgInfo
{
    public Attack attack;
    //who dealt this damage
    public readonly Transform owner;
    //relevant for block angles
    public Vector3 ContactPoint;
    public float damage
    {
        get
        {
            return attack.damage;
        }
    }
    public AttackStrength strength
    {
        get
        {
            return attack.strength;
        }
    }
    public AttackType attackType
    {
        get
        {
            return attack.type;
        }
    }
    public DmgInfo(Transform owner)
    {
        this.owner = owner;
    }
}