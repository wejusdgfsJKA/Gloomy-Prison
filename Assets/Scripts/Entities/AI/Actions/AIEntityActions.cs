using UnityEngine;
//we might have a stationary AI, so no movement here
public class AIEntityActions : EntityActions
{
    public void FaceTarget(Vector3 _pos)
    {
        transform.LookAt(new Vector3(_pos.x, transform.position.y, _pos.z));
    }
}
