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
    //we will have two points for a capsule or one for a box
    public Transform[] points = new Transform[2];
    //if we have a capsule, the first float will be the radius; if we have a box,
    //we will have three floats which will represent the extents
    public float[] floats = new float[3];
    protected System.Func<int> Func;
    public int CheckVolume()
    {
        if (Func == null)
        {
            if (points.Length > 1)
            {
                //we have a capsule
                Func = CheckCapsuleVolume;
            }
            else
            {
                //we have a box
                Func = CheckBoxVolume;
            }
        }
        return Func();
    }
    protected int CheckCapsuleVolume()
    {
        //checks a capsule volume
        return Physics.OverlapCapsuleNonAlloc(points[0].position,
            points[1].position, floats[0], hits, mask);
    }
    protected int CheckBoxVolume()
    {
        //checks a box-shaped volume
        return Physics.OverlapBoxNonAlloc(points[0].position, new Vector3(floats[0],
            floats[1], floats[2]), hits, points[0].rotation, mask);
    }
}