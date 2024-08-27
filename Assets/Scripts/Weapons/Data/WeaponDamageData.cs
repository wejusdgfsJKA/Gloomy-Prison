using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponData")]
[System.Serializable]
public class WeaponDamageData : ScriptableObject
{
    [field: SerializeField]
    public float CheckInterval { get; } = 0.01f;//how often should the
    //hurtboxes check for collisions; 0.01f seems to be reliable, anything
    //slower will cause quick animations to sometimes not register hits
    [field: SerializeField]
    public int MaxHurtboxEntities { get; } = 10;
    //the enemy's layermask
    [field: SerializeField]
    public LayerMask Mask { get; }
    //used for the AI to judge distances
    [field: SerializeField]
    public float Reach { get; }
}