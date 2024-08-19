using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/EntityData")]
public class EntityData : ScriptableObject
{
    public string Name;
    public float hp, poise, stamina, movespeed;
    public EntityBase prefab;
}
