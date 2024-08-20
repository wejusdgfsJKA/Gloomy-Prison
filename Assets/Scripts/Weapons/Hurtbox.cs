using System.Collections.Generic;
using UnityEngine;
//wrapper class for a more reliable hitbox than a trigger collider
[System.Serializable]
public class Hurtbox
{
    //basic capsule shaped hurtbox
    [HideInInspector]
    public int maxEntities;
    [HideInInspector]
    public LayerMask mask;
    public Collider[] hits;
    [SerializeField]
    protected Collider collider;
    protected CapsuleCollider capsule;
    protected BoxCollider box;
    public LinkedList<Vector3> PreviousPositions { get; protected set; } = null;
    [SerializeField]
    protected int NumberOfPreviouses = 2;
    public void Init(LayerMask mask, int MaxEntities)
    {
        this.mask = mask;
        this.maxEntities = MaxEntities;
        this.hits = new Collider[MaxEntities];
        if (collider is CapsuleCollider)
        {
            capsule = (CapsuleCollider)collider;
            return;
        }
        if (collider is BoxCollider)
        {
            box = (BoxCollider)collider;
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
        if (NumberOfPreviouses > 0)
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
            PreviousPositions.AddLast(collider.transform.position);
            while (PreviousPositions.Count > NumberOfPreviouses)
            {
                PreviousPositions.RemoveFirst();
            }
        }
    }
    public int CheckVolume()
    {
        if (box != null)
        {
            //this represents a box collider
            return Physics.OverlapBoxNonAlloc(box.center, box.size / 2, hits,
                box.transform.rotation, mask);
        }
        if (capsule != null)
        {
            //this represents a capsule collider
            Vector3 p1 = capsule.center;
            Vector3 p2 = capsule.center;
            switch (capsule.direction)
            {
                case 0:
                    //the capsule is oriented on the X axis
                    p1 += capsule.transform.right * (capsule.height / 2 -
                        capsule.radius);
                    p2 -= capsule.transform.right * (capsule.height / 2 -
                        capsule.radius);
                    break;
                case 1:
                    //the capsule is oriented on the Y axis
                    p1 += capsule.transform.up * (capsule.height / 2 -
                        capsule.radius);
                    p2 -= capsule.transform.up * (capsule.height / 2 -
                        capsule.radius);
                    break;
                case 2:
                    //the capsule is oriented on the Z axis
                    p1 += capsule.transform.forward * (capsule.height / 2 -
                        capsule.radius);
                    p2 -= capsule.transform.forward * (capsule.height / 2 -
                        capsule.radius);
                    break;
            }
            return Physics.OverlapCapsuleNonAlloc(p1, p2, capsule.radius, hits, mask);
        }
        return 0;
    }
    public Collider GetCollider()
    {
        return collider;
    }
}