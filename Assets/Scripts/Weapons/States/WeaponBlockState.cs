using UnityEngine;

public class WeaponBlockState : WeaponState
{
    public WeaponBlockState(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public override void OnEnter()
    {
        weapon.UsedFeint = false;
        weapon.UsedAlt = false;
        weapon.TimeStartedBlocking = Time.time;
    }
    public override void OnExit()
    {
        weapon.TimeStartedBlocking = -1;
    }
}