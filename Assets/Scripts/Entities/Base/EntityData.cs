using UnityEngine;
/// <summary>
/// Contains all the relevant information about an entity.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/EntityData")]
[System.Serializable]
public class EntityData : ScriptableObject
{
    /// <summary>
    /// Internal ID of the entity.
    /// </summary>
    [field: SerializeField]
    public int ID { get; protected set; }
    /// <summary>
    /// The prefab that will be used for instantiation.
    /// </summary>
    [field: SerializeField]
    public EntityBase Prefab { get; protected set; }
    [field: SerializeField]
    public int MaxHp { get; protected set; }
    [field: SerializeField]
    public int MaxStamina { get; protected set; }
}
