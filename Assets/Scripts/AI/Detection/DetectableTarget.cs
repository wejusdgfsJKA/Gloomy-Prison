using UnityEngine;

public class DetectableTarget : MonoBehaviour
{
    private void OnEnable()
    {
        DetectionManager.Instance.RegisterTarget(transform.root);
    }
    private void OnDisable()
    {
        DetectionManager.Instance.DeRegisterTarget(transform.root);
    }
}
