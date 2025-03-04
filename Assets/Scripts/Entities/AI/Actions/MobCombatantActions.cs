//the actions of a mobile AI which can engage in regular combat
public class MobCombatantActions : MobActions
{
    public EntityBase GetEntity
    {
        get
        {
            return entity;
        }
    }
    public void PerformAttack(Attack.Type type)
    {
        entity.CurrentWeapon.PerformAttack(type);
    }
    public void Block(bool block)
    {
        entity.CurrentWeapon.Block(block);
    }
}
