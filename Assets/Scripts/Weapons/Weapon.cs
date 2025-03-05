using Animancer;
using System.Collections.Generic;
using UnityEngine;

//base weapon class, handles animations and states
[RequireComponent(typeof(WeaponDamageComponent))]
[RequireComponent(typeof(StaminaComponent))]
[RequireComponent(typeof(AnimancerComponent))]
public class Weapon : MonoBehaviour
{
    protected WeaponState currentState;
    protected WeaponIdleState idleState;
    protected WeaponWindupState windupState;
    protected WeaponReleaseState releaseState;
    protected WeaponRecoveryState recoveryState;
    protected WeaponBlockState blockState;
    /// <summary>
    /// When we started blocking. If -1, means we are not blocking.
    /// </summary>
    public float TimeStartedBlocking { get; set; } = -1;
    /// <summary>
    /// The state the weapon is currently in. DO NOT SET THIS IN CODE!!!!
    /// ONLY IN ANIMATION EVENTS!!!!
    /// </summary>
    public WeaponState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            if (currentState != value)
            {
                currentState.OnExit();
                currentState = value;
                currentState.OnEnter();
            }
        }
    }
    [SerializeField]
    protected WeaponData weaponData;
    /// <summary>
    /// The weapon's DamageComponent.
    /// </summary>
    public WeaponDamageComponent DamageComp { get; protected set; }
    /// <summary>
    /// The weapon's BlockComponent.
    /// </summary>
    public WeaponBlockComponent BlockComp { get; protected set; }
    protected AnimancerComponent animancerComponent;
    protected StaminaComponent staminaComponent;
    /// <summary>
    /// A reference to the wielder's stamina component.
    /// </summary>
    public StaminaComponent StaminaComp
    {
        get
        {
            return staminaComponent;
        }
        set
        {
            staminaComponent = value;
            BlockComp.StaminaComp = staminaComponent;
        }
    }
    protected Dictionary<Attack.Type, Attack> attacks;
    /// <summary>
    /// The last attack this weapon has performed.
    /// </summary>
    public Attack LastAttack
    {
        get
        {
            return DamageComp.CurrentAttack;
        }
        protected set
        {
            if (LastAttack != value)
            {
                LastAttack = DamageComp.CurrentAttack = value;
            }
        }
    }
    /// <summary>
    /// How far the weapon can reach.
    /// </summary>
    public float Reach
    {
        get
        {
            return weaponData.Reach;
        }
    }
    /// <summary>
    /// If we previously used the alternate version of an attack. Used for combo logic.
    /// </summary>
    public bool UsedAlt { get; set; }
    /// <summary>
    /// If false, we can perform a feint.
    /// </summary>
    public bool UsedFeint { get; set; }
    protected void Awake()
    {
        //initialize the states of the weapon
        {
            idleState = new WeaponIdleState(this);
            windupState = new WeaponWindupState(this);
            releaseState = new WeaponReleaseState(this);
            recoveryState = new WeaponRecoveryState(this);
            blockState = new WeaponBlockState(this);

            currentState = idleState;
            currentState.OnEnter();
        }

        //get all the necessary components
        {
            animancerComponent = GetComponent<AnimancerComponent>();
            DamageComp = GetComponent<WeaponDamageComponent>();
            BlockComp = GetComponent<WeaponBlockComponent>();
        }

        //internalize animations in a dictionary for ease of access
        {
            attacks = new();
            foreach (Attack atk in DamageComp.Attacks)
            {
                attacks.Add(atk.AttackType, atk);
            }
        }
    }
    protected void OnEnable()
    {
        ReturnToIdle();
    }
    /// <summary>
    /// Check if we successfully blocked an attack. Drain stamina accordingly.
    /// </summary>
    /// <param name="dmgInfo">The damage package.</param>
    /// <returns>A BlockResult type.</returns>
    public BlockResult Block(DmgInfo dmgInfo)
    {
        if (currentState is WeaponBlockState)
        {
            var result = BlockComp.Block(dmgInfo);
            if (result == BlockResult.Success)
            {
                staminaComponent.DrainStamina(dmgInfo.Attack.StaminaDamage);
                if (dmgInfo.Attack.AttackStrength == Attack.Strength.Heavy && !weaponData.Shield)
                {
                    return BlockResult.Partial;
                }
            }
            return result;
        }
        return BlockResult.Failure;
    }
    /// <summary>
    /// Start or stop blocking.
    /// </summary>
    /// <param name="block">Start blocking if true, stop blocking otherwise.</param>
    public void Block(bool block)
    {
        if (block)
        {
            if (CurrentState.CanBlock)
            {
                if (CurrentState is WeaponWindupState && UsedFeint)
                {
                    return;
                }
                animancerComponent.Play(weaponData.BlockAnim, .25f);
            }
            return;
        }
        if (CurrentState is WeaponBlockState)
        {
            ReturnToIdle();
        }
    }
    /// <summary>
    /// Perform an attack animation.
    /// </summary>
    /// <param name="type">The type of attack to perform.</param>
    /// <param name="alt">If true, perform the alternate version of the attack.</param>
    public void PerformAttack(Attack.Type type, bool alt = false)
    {
        if (CurrentState.CanAttack)
        {
            UsedAlt = alt;
            if (CurrentState is WeaponRecoveryState)
            {
                if (LastAttack.AttackType == type)
                {
                    //perform a combo, WIP
                    PerformAttack(attacks[type], !UsedAlt);
                    return;
                }
            }
            PerformAttack(attacks[type], alt);
        }
    }
    protected AnimancerState PerformAttack(Attack attack, bool alt)
    {
        UsedAlt = alt;
        LastAttack = attack;
        AnimancerState state;
        if (alt)
        {
            state = animancerComponent.Play(attack.Alternate, .25f);
        }
        else
        {
            state = animancerComponent.Play(attack.Regular, .25f);
        }
        state.Events(this).OnEnd ??= ReturnToIdle;
        //perform lunge
        return state;
    }
    /// <summary>
    /// Return to idle if not in release state.
    /// </summary>
    protected void ReturnToIdle()
    {
        if (!(currentState is WeaponReleaseState))
        {
            animancerComponent.Play(weaponData.IdleAnim);
        }
    }
    /// <summary>
    /// Return to an idle state no matter what.
    /// </summary>
    public void Interrupt()
    {
        animancerComponent.Play(weaponData.IdleAnim);
    }
}