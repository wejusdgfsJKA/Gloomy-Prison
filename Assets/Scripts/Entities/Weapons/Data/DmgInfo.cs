using UnityEngine;

[System.Serializable]
public class DmgInfo
{
    public Attack Attack { get; set; }
    //who dealt this damage
    public readonly Transform Owner;
    public Vector3 ContactPoint { get; set; }
    public DmgInfo(Transform _owner)
    {
        Owner = _owner;
    }
}