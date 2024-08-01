using UnityEditor;
using UnityEngine;
public class EntityBase : MonoBehaviour
{
    //the state the entity is currently in; will likely be used for the AI
    //to respond to the player's actions
    public enum EntityState { Attacking, Blocking, Idle, Pushing, Winding }
    public EntityState CurrentState { get; protected set; }
    public EntityData entityData;
    [SerializeField]
    protected float hp;
    [SerializeField]
    protected float stamina;
    [SerializeField]
    protected float poise;
    [SerializeField]
    protected Weapon CurrentWeapon;
    [SerializeField]
    protected bool ShowDebug;
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
        var blockResult = CurrentWeapon.Block(dmgInfo);
        switch (blockResult)
        {
            case BlockResult.Perfect:
                //we executed a timed block
                DealStaminaDamage(dmgInfo.staminaCost * Settings.instance.TimedBlockStaminaCostCoefficient);
                break;
            case BlockResult.Failed:
                //we failed to block
                DealDamage(dmgInfo.damage);
                DealPoiseDamage(dmgInfo.poiseDamage);
                break;
            case BlockResult.Success:
                //we successfully blocked
                DealStaminaDamage(dmgInfo.staminaCost);
                break;
            case BlockResult.Partial:
                //we tried to block a heavy attack without a shield
                DealDamage(dmgInfo.damage);
                DealPoiseDamage(dmgInfo.poiseDamage);
                DealStaminaDamage(dmgInfo.staminaCost);
                break;
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
    //the following are just getters for the editor scripts
    public float GetStrikeBlockAngle()
    {
        return CurrentWeapon.GetStrikeBlockAngle();
    }
    public float GetThrustBlockAngle()
    {
        return CurrentWeapon.GetThrustBlockAngle();
    }
    public bool HasWeapon()
    {
        //used so the debug script doesn't have a stroke when selecting an unarmed entity
        return CurrentWeapon != null;
    }
    public Transform GetBlockPoint()
    {
        return CurrentWeapon.GetBlockPoint();
    }
    public bool GetShowDebug()
    {
        return ShowDebug;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(EntityBase), true)]
public class EntityDebug : Editor
{
    protected void OnSceneGUI()
    {
        EntityBase entity = (EntityBase)target;
        if (entity.GetShowDebug() && entity.HasWeapon() && entity.GetBlockPoint() != null)
        {
            Handles.color = Color.green;
            Handles.DrawWireArc(entity.GetBlockPoint().position, Vector3.up,
                entity.GetBlockPoint().forward, 360, 2);
            //show the angle at which we'll block a strike in yellow
            {
                float angle = Mathf.Deg2Rad * (90 - entity.GetStrikeBlockAngle());
                Vector3 p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward + Mathf.Cos(angle) * 2 * entity.GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.GetBlockPoint().
                    forward - Mathf.Cos(angle) * 2 * entity.GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
            }
            //show the angle at which we'll block a thrust in red
            {
                Handles.color = Color.red;
                float angle = Mathf.Deg2Rad * (90 - entity.GetThrustBlockAngle());
                Vector3 p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward + Mathf.Cos(angle) * 2 * entity.GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.GetBlockPoint().
                    forward - Mathf.Cos(angle) * 2 * entity.GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
            }
        }
    }
}
#endif