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
            CurrentWeapon?.PerformAttack(Attack.Type.Light);
        }
    }
    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentWeapon?.PerformAttack(Attack.Type.Thrust);
        }
    }
    public void OnOverhead(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentWeapon?.PerformAttack(Attack.Type.Overhead);
        }
    }
    public void OnChargeUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {

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
    public void OnKick(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
    }
    public void OnShove(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
    }
}
