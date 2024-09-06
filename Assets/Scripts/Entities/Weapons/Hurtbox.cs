using System;
using System.Collections.Generic;
using UnityEngine;
//wrapper class for a more reliable hitbox than a trigger collider
[System.Serializable]
public class Hurtbox
{
    //basic capsule shaped hurtbox
    public int MaxEntities { get; protected set; }
    public LayerMask Mask { get; protected set; }
    public Collider[] Hits { get; protected set; }
    [field: SerializeField]
    public Collider Collider { get; protected set; }
    protected CapsuleCollider capsule;
    protected BoxCollider box;
    public LinkedList<Vector3> PreviousPositions { get; protected set; } = null;
    [SerializeField]
    protected int numberOfPreviouses = 2;
    public void Init(LayerMask mask, int maxEntities)
    {
        Mask = mask;
        MaxEntities = maxEntities;
        Hits = new Collider[MaxEntities];
        if (Collider is CapsuleCollider)
        {
            capsule = (CapsuleCollider)Collider;
            return;
        }
        if (Collider is BoxCollider)
        {
            box = (BoxCollider)Collider;
            return;
        }
    }
    public void ResetPreviouses()
    {
        if (PreviousPositions == null)
        {
            PreviousPositions = new LinkedList<Vector3>();
        }
        else
        {
            PreviousPositions.Clear();
        }
    }
    public void AddPrevious()
    {
        if (numberOfPreviouses > 0)
        {
            if (PreviousPositions == null)
            {
                PreviousPositions = new LinkedList<Vector3>();
            }
            /*if (PreviousPositions.Count > 0)
            {
                var v = PreviousPositions.Last.Value;
                Debug.DrawLine(v, vector3, Color.green, 5);
            }*/
            PreviousPositions.AddLast(Collider.transform.position);
            if (PreviousPositions.Count > numberOfPreviouses)
            {
                PreviousPositions.RemoveFirst();
            }
        }
    }
    public int CheckVolume()
    {
        Array.Clear(Hits, 0, Hits.Length);
        BoxCollider box = (BoxCollider)Collider;
        if (box != null)
        {
            //this is a box collider
            return Physics.OverlapBoxNonAlloc(box.center, box.size / 2, Hits, box.transform.rotation, Mask);
        }
        if (capsule != null)
        {
            //this is a capsule collider
            Vector3 p1 = capsule.center;
            Vector3 p2 = capsule.center;
            switch (capsule.direction)
            {
                case 0:
                    //the capsule is oriented on the X axis
                    p1 += capsule.transform.right * (capsule.height / 2 - capsule.radius);
                    p2 -= capsule.transform.right * (capsule.height / 2 - capsule.radius);
                    break;
                case 1:
                    //the capsule is oriented on the Y axis
                    p1 += capsule.transform.up * (capsule.height / 2 - capsule.radius);
                    p2 -= capsule.transform.up * (capsule.height / 2 - capsule.radius);
                    break;
                case 2:
                    //the capsule is oriented on the Z axis
                    p1 += capsule.transform.forward * (capsule.height / 2 - capsule.radius);
                    p2 -= capsule.transform.forward * (capsule.height / 2 - capsule.radius);
                    break;
            }
            return Physics.OverlapCapsuleNonAlloc(p1, p2, capsule.radius, Hits, Mask);
        }
        return 0;
    }
}