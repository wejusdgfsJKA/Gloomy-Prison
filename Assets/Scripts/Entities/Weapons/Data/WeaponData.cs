using Animancer;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponData")]
[System.Serializable]
public class WeaponData : ScriptableObject
{
    //used for the AI to judge distances
    [field: SerializeField]
    public float Reach { get; protected set; }
    [field: SerializeField]
    public float JabReach { get; protected set; }

    [field: SerializeField]
    public bool Shield { get; protected set; } = false;
    [field: SerializeField]
    public ClipTransition BlockAnim { get; protected set; }
    [field: SerializeField]
    public ClipTransition IdleAnim { get; protected set; }
}
