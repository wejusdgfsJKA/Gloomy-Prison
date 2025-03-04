public class WeaponIdleState : WeaponState
{
    public WeaponIdleState(Weapon weapon)
    {
        this.weapon = weapon;
    }
    public override void OnEnter()
    {
        weapon.UsedFeint = false;
        weapon.UsedAlt = false;
    }
}