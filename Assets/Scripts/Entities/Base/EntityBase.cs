using UnityEngine;
[RequireComponent(typeof(HPComponent))]
public class EntityBase : MonoBehaviour
{
    //is the connection between all the other components
    [field: SerializeField]
    public Weapon CurrentWeapon { get; protected set; }
    protected HPComponent hpComponent;
    protected StaminaComponent staminaComponent
    {
        get
        {
            return CurrentWeapon.StaminaComp;
        }
    }
    protected EntityActions actions;
    [SerializeField]
    protected EntityData data;
    protected BlockResult defaultBlockResult = BlockResult.Failure;
    public string Name
    {
        get
        {
            return data.Name;
        }
    }
    protected virtual void Awake()
    {
        hpComponent = GetComponent<HPComponent>();
        hpComponent.SetMaxHP(data.MaxHp);
        actions = GetComponent<EntityActions>();
        if (CurrentWeapon != null)
        {
            staminaComponent.SetMaxStamina(data.MaxStamina);
        }
    }
    protected virtual void OnEnable()
    {
        hpComponent.Reset();
        staminaComponent.Reset();
    }
    public void ReceiveAttack(DmgInfo _dmginfo)
    {
        BlockResult _blockresult;
        if (CurrentWeapon != null)
        {
            _blockresult = CurrentWeapon.Block(_dmginfo);
        }
        else
        {
            //we had no weapon, so we return the default block result
            _blockresult = defaultBlockResult;
        }
        EntityManager.Instance.SendAttackResult(_blockresult, _dmginfo);
        switch (_blockresult)
        {
            //WIP
            case BlockResult.Failure:
                hpComponent.TakeDamage(_dmginfo.Attack.Damage);
                break;
            case BlockResult.Partial:
                hpComponent.TakeDamage(_dmginfo.Attack.Damage);
                staminaComponent.DrainStamina(_dmginfo.Attack.StaminaDamage);
                break;
            case BlockResult.Success:
                staminaComponent.DrainStamina(_dmginfo.Attack.StaminaDamage);
                break;
        }
    }
    public virtual void ReceiveAttackResult(BlockResult _blockResult, DmgInfo _dmginfo)
    {

    }
}
