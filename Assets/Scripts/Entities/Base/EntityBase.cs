using UnityEngine;
[RequireComponent(typeof(HPComponent))]
public class EntityBase : MonoBehaviour
{
    //is the connection between all the other components
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
            }
        }
    }
    public StaminaComponent StaminaComp { get; protected set; }
    protected HPComponent hpComponent;
    [SerializeField]
    protected EntityData data;
    protected BlockResult defaultBlockResult = BlockResult.Failure;
    public int ID
    {
        get
        {
            return data.ID;
        }
    }
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
    protected virtual void Awake()
    {
        StaminaComp = GetComponent<StaminaComponent>();
        StaminaComp?.SetMaxStamina(data.MaxStamina);
        hpComponent = GetComponent<HPComponent>();
        hpComponent.SetMaxHP(data.MaxHp);
    }
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
        EntityManager.Instance.SendAttackResult(blockresult, dmginfo);
        switch (blockresult)
        {
            //WIP
            case BlockResult.Failure:
                var a = this;
                hpComponent.TakeDamage(dmginfo.Attack.Damage);
                break;
            case BlockResult.Partial:
                hpComponent.TakeDamage(dmginfo.Attack.Damage);
                break;
        }
    }
    public virtual void ReceiveAttackResult(BlockResult _blockResult, DmgInfo _dmginfo)
    {

    }
}
