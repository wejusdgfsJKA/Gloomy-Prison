using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponData")]
[System.Serializable]
public class WeaponBlockData : ScriptableObject
{
    public float BlockAngle { get; }
    [field: SerializeField]
    public bool Shield { get; } = false;
}