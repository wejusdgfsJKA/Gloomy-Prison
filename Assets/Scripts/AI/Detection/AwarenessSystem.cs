using UnityEngine;

public class AwarenessSystem : MonoBehaviour
{
    public Transform target;
    private void OnEnable()
    {
        target = null;
    }
    public void ReportHasSeen(Transform target)
    {
        //the vision sensor has seen this target
        Debug.DrawLine(transform.position, target.position, Color.red);
        /*if (this.target == null || !this.target.gameObject.activeSelf)
        {
            //we have no previous target
            this.target = target;
        }
        else
        {
            if (Vector3.Distance(transform.position, target.position) <
                Vector3.Distance(transform.position, this.target.position))
            {
                //this target is closer
                this.target = target;
            }
        }*/
    }
    public void ReportInProximity(Transform target)
    {
        //something is right next to us
        Debug.DrawLine(transform.position, target.position, Color.cyan);
    }
    public void ReportHasHeard(Sound sound)
    {
        //we've heard a noise
        Debug.DrawLine(transform.position, sound.data.source.position, Color.magenta);
    }
}
