using System.Threading.Tasks;
using UnityEngine;
//a partial block refers to blocking a heavy attack without a shield
public enum BlockResult { Success = 0, Failure = 1, Partial = 2 }
[RequireComponent(typeof(WeaponDamageComponent))]
[RequireComponent(typeof(StaminaComponent))]
public class Weapon : MonoBehaviour
{
    //base weapon class, handles animations and states
    //these states will be used by the AI to make decisions
    public enum AnimState
    {
        Idle = 0, Winding = 1, Attacking = 2, Blocking = 3, Recovering = 4, Staggered = 5
    }
    protected Animator animator;
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
                        damageComponent.ExitDamageState();
                        feinted = false;
                        usedaltattack = false;
                        lastUsedAttack = null;
                        break;
                    case AnimState.Winding:
                        break;
                    case AnimState.Attacking:
                        damageComponent.EnterDamageState();
                        feinted = false;
                        break;
                    case AnimState.Blocking:
                        feinted = false;
                        usedaltattack = false;
                        lastUsedAttack = null;
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
    //we cannot feint more than once
    protected bool feinted;
    //attacks in a combo will alternate between normal and alternate directions
    protected bool usedaltattack;
    public bool Riposting { get; protected set; }
    public bool Countering { get; protected set; }
    public StaminaComponent staminaComponent { get; protected set; }
    public Attack[] Attacks
    {
        get
        {
            return damageComponent.Attacks;
        }
    }
    protected Attack lastUsedAttack
    {
        get
        {
            return lastUsedAttack;
        }
        set
        {
            lastUsedAttack = value;
            CurrentState.Attack = value;
        }
    }
    protected void Awake()
    {
        animator = GetComponent<Animator>();
        damageComponent = GetComponent<WeaponDamageComponent>();
        blockComponent = GetComponent<WeaponBlockComponent>();
        staminaComponent = GetComponent<StaminaComponent>();
    }
    protected void OnEnable()
    {
        Riposting = false;
        Countering = false;
    }
    public bool TryPerformAttack(Attack.Type type, bool alt)
    {
        if (animState == AnimState.Idle)
        {
            //the first attack from idle is free
            PerformAttack(Attacks[(int)type], alt);
            return true;
        }
        if (animState == AnimState.Recovering)
        {
            if (type == lastUsedAttack.AttackType)
            {
                //we are comboing
                if (Attacks[(int)type].StaminaDrain <= staminaComponent.CurrentStamina)
                {
                    //we have enough stamina for this attack
                    PerformCombo();
                    return true;
                }
                return false;
            }
            return false;
        }
        if (animState == AnimState.Winding)
        {
            //we are feinting
            if (!feinted)
            {
                Attack attack = Attacks[(int)type];
                if (attack.StaminaDrain <= staminaComponent.CurrentStamina)
                {
                    //we have enough stamina for this attack
                    PerformFeint(attack, alt);
                    return true;
                }
            }
            return false;
        }
        return false;
    }
    protected void PerformAttack(Attack attack, bool alt)
    {

    }
    protected void PerformCombo()
    {
        staminaComponent.DrainStamina(lastUsedAttack.StaminaDrain);
    }
    protected void PerformFeint(Attack attack, bool alt)
    {
        staminaComponent.DrainStamina(attack.StaminaDrain);
    }
    protected async void PerformRiposte(Attack attack, bool alt)
    {
        Riposting = true;
        await Task.Delay(50);
        Riposting = false;
    }
    protected async void PerformCounter(Attack attack, bool alt)
    {
        Countering = true;
        await Task.Delay(50);
        Countering = false;
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
        if (blockComponent != null && (animState == AnimState.Blocking ||
            Riposting || Countering))
        {
            return blockComponent.Block(dmgInfo);
        }
        return BlockResult.Failure;
    }
    public void Interrupt()
    {
        damageComponent.ExitDamageState();
        animState = AnimState.Idle;
    }
}