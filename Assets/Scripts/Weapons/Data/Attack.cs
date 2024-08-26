using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/AttackData")]
[System.Serializable]
public class Attack : ScriptableObject
{
    [field: SerializeField]
    public float damage { get; }
    [field: SerializeField]
    public float staminaCost { get; }
    [field: SerializeField]
    public float selfStaminaCost { get; }
    [field: SerializeField]
    public AttackType type { get; }
    [field: SerializeField]
    public AttackStrength strength { get; }
}
