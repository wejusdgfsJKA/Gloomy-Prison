using UnityEditor;
using UnityEngine;
public enum EntityState { Striking, Thrusting, Winding, Blocking, Idle, Pushing }
[System.Serializable]
public class EntityBase : MonoBehaviour
{
    //the state the entity is currently in; will likely be used for the AI
    //to respond to the player's actions
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
    protected bool ShowStrikeBlockAngle, ShowThrustBlockAngle;
    protected AnimHandler animHandler;
    [HideInInspector]
    public bool Damaged, DealtDamage;
    protected virtual void Awake()
    {
        animHandler = GetComponentInChildren<AnimHandler>();
    }
    protected virtual void OnEnable()
    {
        //reset all stats and enter idle state
        hp = entityData.hp;
        stamina = entityData.stamina;
        poise = entityData.poise;
        CurrentWeapon.StopBlocking();
        Damaged = false;
    }
    public EntityState ReturnCurrentState()
    {
        if (CurrentWeapon.StartedBlocking > 0)
        {
            return EntityState.Blocking;
        }
        if (CurrentWeapon.enabled)
        {
            switch (CurrentWeapon.GetCurrentAttackType())
            {
                case AttackType.Push:
                    return EntityState.Pushing;
                case AttackType.Thrust:
                    return EntityState.Thrusting;
                case AttackType.Strike:
                    return EntityState.Striking;
            }
        }
        if (animHandler.animator.GetBool("Winding"))
        {
            return EntityState.Winding;
        }
        return EntityState.Idle;
    }
    public virtual bool IsThreat(Transform t)
    {
        if (Vector3.Distance(transform.position, t.position) > CurrentWeapon.GetReach() * 2)
        {
            return false;
        }
        if (CurrentWeapon.enabled || animHandler.animator.GetBool("Winding"))
        {
            return true;
        }
        return false;
    }
    public virtual void BeginBlocking()
    {
        //this entity just started blocking
        //useful for determining timed blocks

    }
    public virtual void StopBlocking()
    {

    }
    public virtual void ReceiveAttack(DmgInfo dmgInfo)
    {
        if (dmgInfo.attackType == AttackType.Push)
        {
            //we were pushed by the enemy
            DealStaminaDamage(dmgInfo.staminaCost);
            DealPoiseDamage(dmgInfo.poiseDamage);
            return;
        }
        var blockResult = CurrentWeapon.Block(dmgInfo);
        //Debug.Log(blockResult, this);
        switch (blockResult)
        {
            case BlockResult.Perfect:
                //we executed a timed block
                DealStaminaDamage(dmgInfo.staminaCost * Settings.instance.TimedBlockStaminaCostCoefficient);
                EntityManager.instance.entities[dmgInfo.owner].PerfectBlocked();
                break;
            case BlockResult.Failed:
                //we failed to block
                DealDamage(dmgInfo.damage);
                EntityManager.instance.entities[dmgInfo.owner].DealtDamage = true;
                DealPoiseDamage(dmgInfo.poiseDamage);
                break;
            case BlockResult.Success:
                //we successfully blocked
                DealStaminaDamage(dmgInfo.staminaCost);
                EntityManager.instance.entities[dmgInfo.owner].Blocked();
                break;
            case BlockResult.Partial:
                //we tried to block a heavy attack without a shield
                DealDamage(dmgInfo.damage);
                EntityManager.instance.entities[dmgInfo.owner].DealtDamage = true;
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
        Damaged = true;
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
    protected void Stagger()
    {
        animHandler.StopSwing();
    }
    public void Blocked()
    {
        //our attack has been blocked
        animHandler.StopSwing();
    }
    public void PerfectBlocked()
    {
        //our attack has been blocked
        animHandler.Stagger();
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
    public float GetReach()
    {
        return CurrentWeapon.GetReach();
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
    public Transform GetWeaponCenter()
    {
        return CurrentWeapon.GetCenter();
    }
    public float GetHP()
    {
        return hp;
    }
    public float GetStamina()
    {
        return stamina;
    }
    public bool WeaponActive()
    {
        return CurrentWeapon.enabled;
    }
    public (bool, bool) GetShowDebug()
    {
        return (ShowStrikeBlockAngle, ShowThrustBlockAngle);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(EntityBase), true)]
public class EntityDebug : Editor
{
    protected void OnSceneGUI()
    {
        EntityBase entity = (EntityBase)target;
        if (entity.HasWeapon() && entity.GetBlockPoint() != null)
        {
            var bools = entity.GetShowDebug();
            if (bools.Item1)
            {
                //show the angle at which we'll block a strike in cyan
                Handles.color = Color.cyan;
                float angle = Mathf.Deg2Rad * (90 - entity.GetStrikeBlockAngle());
                Vector3 p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 *
                    entity.GetBlockPoint().forward + Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward - Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward + Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().up;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward - Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().up;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
            }
            if (bools.Item2)
            {
                //show the angle at which we'll block a thrust in red
                Handles.color = Color.red;
                float angle = Mathf.Deg2Rad * (90 - entity.GetThrustBlockAngle());
                Vector3 p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 *
                    entity.GetBlockPoint().forward + Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward - Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().right;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward + Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().up;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
                p1 = entity.GetBlockPoint().position + Mathf.Sin(angle) * 2 * entity.
                    GetBlockPoint().forward - Mathf.Cos(angle) * 2 * entity.
                    GetBlockPoint().up;
                Handles.DrawLine(entity.GetBlockPoint().position, p1);
            }
        }
    }
}
#endif