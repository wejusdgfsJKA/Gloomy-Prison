using UnityEngine;
[RequireComponent(typeof(HPComponent))]
[RequireComponent(typeof(EntityActions))]
public class EntityBase : MonoBehaviour
{
    //handles basic stuff like taking damage, is the connection between
    //all the other components
    [SerializeField]
    protected Weapon weapon;
    protected HPComponent hpComponent;
    protected EntityActions actions;
    [SerializeField]
    protected EntityData data;
    [SerializeField]
    protected BlockResult DefaultBlockResult = BlockResult.Failure;
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
        hpComponent.SetMaxHP(data.maxhp);
        actions = GetComponent<EntityActions>();
    }
    public void ReceiveAttack(DmgInfo dmgInfo)
    {
        BlockResult blockResult = DefaultBlockResult;
        if (weapon != null)
        {
            //we had no weapon, so we return the default block result
            blockResult = weapon.Block(dmgInfo);
        }
        EntityManager.Instance.SendAttackResult(blockResult, dmgInfo);
        switch (blockResult)
        {
            //WIP
            case BlockResult.Failure:
                hpComponent.TakeDamage(dmgInfo.Attack.Damage);
                break;
            case BlockResult.Partial:
                hpComponent.TakeDamage(dmgInfo.Attack.Damage);
                break;
            case BlockResult.Success:
                break;
        }
    }
    public virtual void ReceiveAttackResult(BlockResult blockResult, DmgInfo dmgInfo)
    {

    }
}
