using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/EntityData")]
[System.Serializable]
public class EntityData : ScriptableObject
{
    [field: SerializeField]
    public int ID { get; protected set; }
    [field: SerializeField]
    public EntityBase Prefab { get; protected set; }
    [field: SerializeField]
    public int MaxHp { get; protected set; }
    [field: SerializeField]
    public int MaxStamina { get; protected set; }
}
