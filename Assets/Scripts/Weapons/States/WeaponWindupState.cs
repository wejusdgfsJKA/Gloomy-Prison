public class WeaponWindupState : WeaponState
{
    public WeaponWindupState(Weapon weapon)
    {
        this.weapon = weapon;
    }
    public override void OnEnter()
    {
        weapon.TimeStartedBlocking = -1;
    }
}