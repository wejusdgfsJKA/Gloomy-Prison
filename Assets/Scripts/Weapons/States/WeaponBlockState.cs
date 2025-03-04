using UnityEngine;
/// <summary>
/// In this state the weapon blocks attacks in a cone in front of the wielder.
/// </summary>
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