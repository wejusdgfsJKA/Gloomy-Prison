/// <summary>
/// The weapon is dealing damage in this state.
/// </summary>
public class WeaponReleaseState : WeaponState
{
    public WeaponReleaseState(Weapon weapon)
    {
        CanAttack = false;
        CanBlock = false;
        this.weapon = weapon;
    }

    public override void OnEnter()
    {
        weapon.UsedFeint = false;
        weapon.DamageComp.EnterDamageState();
    }

    public override void OnExit()
    {
        weapon.DamageComp.ExitDamageState();
    }
}