using UnityEngine;
public class SkeletonDebug : MonoBehaviour
{
    [SerializeField] bool drawLineToRoot;
    [SerializeField] bool draw;
    [SerializeField] float radiusMultiplier = 0.05f;
    private void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < transform.childCount; i++)
            {
                DrawLines(transform.GetChild(i));
                if (drawLineToRoot)
                {
                    Gizmos.DrawLine(transform.position, transform.GetChild(i).
                        position);
                }
            }
            if (drawLineToRoot)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.transform.position, transform.
                    localScale.magnitude * radiusMultiplier);
            }
        }
    }
    void DrawLines(Transform _t)
    {
        Gizmos.DrawSphere(_t.transform.position, _t.localScale.magnitude *
            radiusMultiplier);
        for (int i = 0; i < _t.childCount; i++)
        {
            if (_t.GetChild(i).gameObject.tag != "StopDrawing")
            {
                DrawLines(_t.GetChild(i));
            }
            Gizmos.DrawLine(_t.position, _t.GetChild(i).position);
        }
    }
}
