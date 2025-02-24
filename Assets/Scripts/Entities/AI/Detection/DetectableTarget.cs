using UnityEngine;

[RequireComponent(typeof(EntityBase))]
public class DetectableTarget : MonoBehaviour
{
    protected void OnEnable()
    {
        DetectionManager.Instance.RegisterTarget(transform.root);
    }
    protected void OnDisable()
    {
        DetectionManager.Instance.DeRegisterTarget(transform.root);
    }
}
