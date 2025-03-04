using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    [field: SerializeField] public Weapon CurrentWeapon { get; set; }
    public void OnStrike(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentWeapon?.PerformAttack(Attack.Type.Strike);
        }
    }
    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentWeapon?.PerformAttack(Attack.Type.Thrust);
        }
    }
    public void OnBlock(InputAction.CallbackContext context)
    {
        if (Convert.ToBoolean(context.ReadValue<float>()))
        {
            CurrentWeapon?.Block(true);
        }
        else
        {
            CurrentWeapon?.Block(false);
        }
    }
    public void OnBash(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
    }
}
