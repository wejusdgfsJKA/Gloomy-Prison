using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/EntityData")]
[System.Serializable]
public class EntityData : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public EntityBase prefab { get; private set; }
    [field: SerializeField]
    public float maxhp { get; private set; }
}
