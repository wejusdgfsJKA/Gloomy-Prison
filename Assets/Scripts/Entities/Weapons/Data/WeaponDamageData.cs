using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponDamageData")]
[System.Serializable]
public class WeaponDamageData : ScriptableObject
{
    [field: SerializeField]
    public float CheckInterval { get; protected set; } = 0.01f;//how often should the
    //hurtboxes check for collisions; 0.01f seems to be reliable, anything
    //slower will cause quick animations to sometimes not register hits
    [field: SerializeField]
    public int MaxHurtboxEntities { get; protected set; } = 10;
    //the enemy's layermask
    [field: SerializeField]
    public LayerMask Mask { get; protected set; }
    //used for the AI to judge distances
    [field: SerializeField]
    public float Reach { get; protected set; }
    [field: SerializeField]
    public Attack[] Attacks { get; protected set; }
}