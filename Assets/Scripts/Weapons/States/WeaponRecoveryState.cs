public class WeaponRecoveryState : WeaponState
{
    public WeaponRecoveryState(Weapon weapon)
    {
        CanAttack = true;
        CanBlock = false;
        this.weapon = weapon;
    }
    public override void OnEnter() { }
}