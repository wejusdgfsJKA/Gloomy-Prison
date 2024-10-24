using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponBlockData")]
[System.Serializable]
public class WeaponBlockData : ScriptableObject
{
    [field: SerializeField]
    public float BlockAngle { get; protected set; }
}