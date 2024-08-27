using UnityEngine;
//a partial block refers to blocking a heavy attack without a shield
public enum BlockResult { Success, Failure, Partial }
[RequireComponent(typeof(WeaponDamageComponent))]
public class Weapon : MonoBehaviour
{
    //base weapon class, handles animations and states
    //these states will be used by the AI to make decisions
    public enum AnimState
    {
        Idle, Winding, Attacking, Blocking, Recovering
    }
    protected WeaponDamageComponent damageComponent;
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
                        break;
                    case AnimState.Attacking:
                        damageComponent.EnterDamageState();
                        feinted = false;
                        break;
                    case AnimState.Blocking:
                        feinted = false;
                        break;
                    case AnimState.Recovering:
                        damageComponent.ExitDamageState();
                        feinted = false;
                        break;
                }
                animState = CurrentState.AnimState = value;
            }
        }
    }
    public WeaponState CurrentState { get; protected set; }
    protected bool feinted;
    protected void Awake()
    {
        damageComponent = GetComponent<WeaponDamageComponent>();
        blockComponent = GetComponent<WeaponBlockComponent>();
    }
    protected void OnEnable()
    {
        animState = AnimState.Idle;
    }
    public bool Feint(Attack newattack)
    {
        if (!feinted && animState == AnimState.Winding && damageComponent.
            CurrentAttack.Feintable)
        {
            //change attack
            feinted = true;
            CurrentState.Attack = newattack;
            damageComponent.CurrentAttack = newattack;
            return true;
        }
        return false;
    }
    public bool Cancel()
    {
        if (!feinted && animState == AnimState.Winding && damageComponent.
            CurrentAttack.Cancelable)
        {
            //cancel the current attack
            return true;
        }
        return false;
    }
    public bool BeginBlocking()
    {
        if (animState == AnimState.Idle)
        {
            animState = AnimState.Blocking;
            return true;
        }
        if (animState == AnimState.Winding && !feinted)
        {
            animState = AnimState.Blocking;
            return true;
        }
        return false;
    }
    public BlockResult Block(DmgInfo dmgInfo)
    {
        if (blockComponent != null && animState == AnimState.Blocking)
        {
            return blockComponent.Block(dmgInfo);
        }
        return BlockResult.Failure;
    }
    public void Interrupt()
    {
        damageComponent.ExitDamageState();
    }
}