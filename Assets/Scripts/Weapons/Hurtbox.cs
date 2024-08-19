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
    public Collider collider;
    public LinkedList<Vector3> PreviousPositions { get; protected set; } = null;
    [SerializeField]
    protected int NumberOfPreviouses = 2;
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
    public void AddPrevious(Vector3 vector3)
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
            PreviousPositions.AddLast(vector3);
            while (PreviousPositions.Count > NumberOfPreviouses)
            {
                PreviousPositions.RemoveFirst();
            }
        }
    }
    public int CheckVolume()
    {
        CapsuleCollider capsule = (CapsuleCollider)collider;
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
            return Physics.OverlapCapsuleNonAlloc(p1, p2, capsule.radius, hits, mask);
        }
        BoxCollider box = (BoxCollider)collider;
        if (box != null)
        {
            //this is a box collider
            return Physics.OverlapBoxNonAlloc(box.center, box.size / 2, hits, box.transform.rotation, mask);
        }
        return 0;
    }
}