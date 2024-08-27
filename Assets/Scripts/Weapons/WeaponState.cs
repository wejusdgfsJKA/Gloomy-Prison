using UnityEngine;

public class WeaponState : MonoBehaviour
{
    public Weapon.AnimState AnimState { get; set; }
    public Attack Attack { get; set; }
    public WeaponState()
    {
        AnimState = Weapon.AnimState.Idle;
    }
}
