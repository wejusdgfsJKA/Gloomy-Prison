using UnityEngine;
//a partial block refers to blocking a heavy attack without a shield
public enum BlockResult { Success, Failure, Partial }
[RequireComponent(typeof(WeaponDamageComponent))]
[RequireComponent(typeof(WeaponBlockComponent))]
public class Weapon : MonoBehaviour
{
    //base weapon class, handles animations and states
    //this states will be used by the AI to make decisions
    public enum AnimState
    {
        Idle, Winding, Attacking, Blocking
    }
    [SerializeField]
    protected WeaponDamageComponent damageComponent;
    [SerializeField]
    protected WeaponBlockComponent blockComponent;
    [field: SerializeField]
    public Transform CenterPoint { get; protected set; }
    //the center will be used
    //for the AI to know in which direction to block
    protected AnimState animState
    {
        get
        {
            return animState;
        }
        set
        {
            if (value != animState)
            {
                switch (animState)
                {
                    case AnimState.Idle:
                        damageComponent.ExitDamageState();
                        break;
                    case AnimState.Attacking:
                        damageComponent.EnterDamageState();
                        feinted = false;
                        break;
                    case AnimState.Blocking:
                        feinted = false;
                        break;
                }
                animState = CurrentState.AnimState = value;
            }
        }
    }
    public WeaponState CurrentState
    {
        get; protected set;
    }
    protected bool feinted;
    protected void OnEnable()
    {
        animState = AnimState.Idle;
    }
    protected void Feint(Attack newattack)
    {
        if (!feinted && animState == AnimState.Winding)
        {
            //change attack
            feinted = true;
            CurrentState.Attack = newattack;
            damageComponent.CurrentAttack = newattack;
        }
    }
    public void Cancel()
    {
        if (!feinted && animState == AnimState.Winding)
        {
            //cancel the current attack
        }
    }
    public BlockResult Block(DmgInfo dmgInfo)
    {
        if (animState == AnimState.Blocking)
        {
            return blockComponent.Block(dmgInfo);
        }
        return BlockResult.Failure;
    }
}