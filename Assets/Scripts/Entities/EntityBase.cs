using UnityEngine;

public class EntityBase : MonoBehaviour
{
    //the state the entity is currently in; will likely be used for the AI
    //to respond to the player's actions
    public enum EntityState { Attacking, Blocking, Idle }
    public EntityState CurrentState { get; protected set; }
    public EntityData entityData;
    [SerializeField]
    protected float hp;
    [SerializeField]
    protected float stamina;
    [SerializeField]
    protected float poise;
    protected float StartedBlocking;
    [SerializeField]
    protected Weapon CurrentWeapon;
    protected virtual void OnEnable()
    {
        //reset all stats and enter idle state
        CurrentState = EntityState.Idle;
        hp = entityData.hp;
        stamina = entityData.stamina;
        poise = entityData.poise;
    }
    public float GetHP()
    {
        return hp;
    }
    public float GetStamina()
    {
        return stamina;
    }
    public void BeginBlocking()
    {
        //this entity just started blocking
        //useful for determining timed blocks
        StartedBlocking = Time.time;
        CurrentState = EntityState.Blocking;
    }
    public void StopBlocking()
    {
        CurrentState = EntityState.Idle;
    }
    public virtual void ReceiveAttack(DmgInfo dmgInfo)
    {
        if (dmgInfo.attackType == DmgInfo.AttackType.Push)
        {
            //we were pushed by the enemy
            DealStaminaDamage(dmgInfo.staminaCost);
            DealPoiseDamage(dmgInfo.poiseDamage);
            return;
        }
        if (!Block(dmgInfo))
        {
            DealDamage(dmgInfo.damage);
            DealPoiseDamage(dmgInfo.poiseDamage);
        }
    }
    public virtual bool Block(DmgInfo dmgInfo)
    {
        if (CurrentState != EntityState.Blocking)
        {
            return false;
        }
        if (Time.time <= StartedBlocking + Settings.instance.TimedBlockWindow)
        {
            //a timed block was executed
            SuccessfullBlock(dmgInfo, true);
            return true;
        }
        //we need to calculate the angle of the strike
        float strikeangle = 0;
        if (dmgInfo.attackType == DmgInfo.AttackType.Strike)
        {
            if (strikeangle <= CurrentWeapon.GetStrikeBlockAngle())
            {
                //we successfully blocked a strike
                SuccessfullBlock(dmgInfo);
                return true;
            }
        }
        else
        {
            if (dmgInfo.attackType == DmgInfo.AttackType.Thrust)
            {
                if (strikeangle <= CurrentWeapon.GetThrustBlockAngle())
                {
                    //we successfully blocked a thrust
                    SuccessfullBlock(dmgInfo);
                    return true;
                }
            }
        }
        return false;
    }
    public virtual void SuccessfullBlock(DmgInfo dmgInfo, bool timed = false)
    {
        if (dmgInfo.strength == DmgInfo.AttackStrength.Regular)
        {
            //we successfully blocked a regular attack
            if (timed)
            {
                //maybe we want to stagger the opponent?
                DealStaminaDamage(dmgInfo.staminaCost *
                    Settings.instance.TimedBlockStaminaCostCoefficient);
                return;
            }
            DealStaminaDamage(dmgInfo.staminaCost);
            return;
        }
        if (dmgInfo.strength == DmgInfo.AttackStrength.Heavy)
        {
            //we need a shield to block this
            if (!CurrentWeapon.GetShieldBool())
            {
                //a heavy blow like this can only be blocked with a shield
                DealPoiseDamage(dmgInfo.poiseDamage);
                DealStaminaDamage(dmgInfo.staminaCost);
                DealDamage(dmgInfo.damage);
                return;
            }
            if (timed)
            {
                //heavy blows like this should not stagger the opponent when timed-blocked
                DealStaminaDamage(dmgInfo.staminaCost *
                    Settings.instance.TimedBlockStaminaCostCoefficient);
                return;
            }
            DealStaminaDamage(dmgInfo.staminaCost);
            return;
        }
    }
    protected virtual void DealStaminaDamage(float staminaDamage)
    {
        stamina -= staminaDamage;
        if (stamina <= 0)
        {
            stamina = 0;
            //I think the entity should be stunned for a moment
        }
    }
    protected virtual void DealDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }
    protected virtual void DealPoiseDamage(float poisedamage)
    {
        poise -= poisedamage;
        if (poise <= 0)
        {
            //stagger
            //reset poise
            Stagger();
        }
    }
    protected virtual void Stagger()
    {
        //this entity's poise was broken
    }
    protected virtual void Die()
    {
        //this entity has been killed
        gameObject.SetActive(false);

    }
    protected virtual void OnDisable()
    {
        EntityManager.instance.Dead(this);
    }
}
