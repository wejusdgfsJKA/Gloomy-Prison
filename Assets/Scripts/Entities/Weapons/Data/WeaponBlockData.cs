using Animancer;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponBlockData")]
[System.Serializable]
public class WeaponBlockData : ScriptableObject
{
    [field: SerializeField]
    public float BlockAngle { get; protected set; }
    [field: SerializeField]
    public bool Shield { get; protected set; } = false;
    [field: SerializeField]
    public ClipTransition BlockAnim { get; protected set; }
}