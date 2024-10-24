using UnityEngine;

public class HurtboxTester : MonoBehaviour
{
    [SerializeField]
    protected Hurtbox hurtbox;
    [SerializeField]
    protected Collider[] hits;
    [SerializeField]
    protected LayerMask layerMask;
    private void Awake()
    {
        hurtbox.Init(layerMask, 10);
    }
    private void Update()
    {
        hits = new Collider[hurtbox.CheckVolume()];
        hits = hurtbox.Hits;
    }
}
