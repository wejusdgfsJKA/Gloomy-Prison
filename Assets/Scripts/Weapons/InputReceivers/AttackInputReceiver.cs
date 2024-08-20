using UnityEngine;

public class AttackInputReceiver : MonoBehaviour
{
    [SerializeField]
    protected Weapon weapon;
    private void OnEnable()
    {
        weapon.EnableCollision();
    }
    private void OnDisable()
    {
        weapon.DisableCollision();
    }
}
