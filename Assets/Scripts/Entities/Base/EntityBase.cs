using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Connects all the other components that make up an entity.
/// </summary>
[RequireComponent(typeof(HPComponent))]
public class EntityBase : MonoBehaviour
{
    [SerializeField]
    protected Weapon weapon;
    public Weapon CurrentWeapon
    {
        get
        {
            return weapon;
        }
        protected set
        {
            weapon = value;
            if (weapon != null)
            {
                weapon.StaminaComp = StaminaComp;
                OnStagger.AddListener(weapon.Interrupt);
            }
        }
    }
    public StaminaComponent StaminaComp { get; protected set; }
    protected HPComponent hpComponent;
    [SerializeField]
    protected EntityData data;
    protected BlockResult defaultBlockResult = BlockResult.Failure;
    /// <summary>
    /// The internal ID of this entity.
    /// </summary>
    public int ID
    {
        get
        {
            return data.ID;
        }
    }
    /// <summary>
    /// How far can the entity reach with its weapon.
    /// </summary>
    public float Reach
    {
        get
        {
            if (CurrentWeapon != null)
            {
                return CurrentWeapon.Reach;
            }
            return 0;
        }
    }
    /// <summary>
    /// Fires when the entity is staggered.
    /// </summary>
    public UnityEvent OnStagger { get; set; }
    protected virtual void Awake()
    {
        StaminaComp = GetComponent<StaminaComponent>();
        StaminaComp?.SetMaxStamina(data.MaxStamina);

        hpComponent = GetComponent<HPComponent>();
        hpComponent.SetMaxHP(data.MaxHp);

        CurrentWeapon = GetComponentInChildren<Weapon>();
    }
    protected virtual void OnEnable()
    {
        EntityManager.Instance.RegisterEntity(this);
    }
    /// <summary>
    /// Receive a damage package, check if we blocked the attack.
    /// </summary>
    /// <param name="dmginfo">The damage package.</param>
    public void ReceiveAttack(DmgInfo dmginfo)
    {
        BlockResult blockresult;
        if (CurrentWeapon != null)
        {
            blockresult = CurrentWeapon.Block(dmginfo);
        }
        else
        {
            //we had no weapon, so we return the default block result
            blockresult = defaultBlockResult;
        }
        EntityManager.Instance.SendAttackResult(transform.root, blockresult, dmginfo);
        switch (blockresult)
        {
            case BlockResult.Success:
                StaminaComp.DrainStamina(dmginfo.Attack.StaminaDamage);
                break;
            case BlockResult.Failure:
                TakeDamage(dmginfo);
                break;
            case BlockResult.Partial:
                //also take stamina damage
                TakeDamage(dmginfo);
                StaminaComp.DrainStamina(dmginfo.Attack.StaminaDamage);
                break;
        }
    }
    /// <summary>
    /// Take damage and, if appropriately, get stunned.
    /// </summary>
    /// <param name="dmginfo">The damage package.</param>
    protected void TakeDamage(DmgInfo dmginfo)
    {
        hpComponent.TakeDamage(dmginfo.Attack.Damage);
        switch (data.StunResist)
        {
            case StunResist.Weak:
                OnStagger?.Invoke();
                break;
            case StunResist.Normal:
                if (dmginfo.Attack.AtkWeight == Attack.Weight.Normal)
                {
                    OnStagger?.Invoke();
                }
                break;
            default:
                OnStagger?.Invoke();
                break;
        }
    }
    public virtual void ReceiveAttackResult(Transform target, BlockResult blockResult, DmgInfo dmginfo)
    {

    }
}
