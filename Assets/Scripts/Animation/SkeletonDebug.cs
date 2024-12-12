using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class SkeletonDebug : Editor
{
    private void OnSceneGUI()
    {
        Transform t = (Transform)target;
        if (t.tag == "AnimSkeleton")
        {
            //this is an animation skeleton
            Handles.color = Color.yellow;
            DrawLines(t);
        }
        else if (t.root.tag == "AnimSkeleton")
        {
            Handles.color = Color.yellow;
            DrawLines(t.root);
        }
    }
    void DrawLines(Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.tag != "AnimSkeleton")
            {
                Handles.DrawLine(t.position, t.GetChild(i).position);
            }
            DrawLines(t.GetChild(i));
        }
    }
}
