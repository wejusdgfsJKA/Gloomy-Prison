using UnityEngine;

public class DetectableTarget : MonoBehaviour
{
    private void OnEnable()
    {
        DetectionManager.instance.RegisterTarget(transform.root);
    }
    private void OnDisable()
    {
        DetectionManager.instance.DeRegisterTarget(transform.root);
    }
}
