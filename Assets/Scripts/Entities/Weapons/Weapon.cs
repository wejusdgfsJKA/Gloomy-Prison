using Animancer;
using System.Collections.Generic;
using UnityEngine;

//base weapon class, handles animations and states
public enum BlockResult { Success = 0, Failure = 1, Partial = 2, Counter = 3 }
[RequireComponent(typeof(WeaponDamageComponent))]
[RequireComponent(typeof(StaminaComponent))]
[RequireComponent(typeof(AnimancerComponent))]
public class Weapon : MonoBehaviour
{
    public enum State { Idle = 0, Windup = 1, Release = 2, Recovery = 3, Blocking = 4 }
    [SerializeField]
    protected State currentState = State.Idle;
    protected float timeStartedBlocking = -1;
    public State CurrentState
    {
        get
        {
            return currentState;
        }
        protected set
        {
            if (currentState != value)
            {
                ChangedState.Invoke();
                switch (value)
                {
                    case State.Idle:
                        usedAlt = false;
                        timeStartedBlocking = -1;
                        damageComponent.ExitDamageState();
                        break;
                    case State.Release:
                        timeStartedBlocking = -1;
                        damageComponent.EnterDamageState();
                        break;
                    case State.Recovery:
                        timeStartedBlocking = -1;
                        damageComponent.ExitDamageState();
                        break;
                    case State.Blocking:
                        usedAlt = false;
                        timeStartedBlocking = Time.time;
                        damageComponent.ExitDamageState();
                        break;
                    case State.Windup:
                        if (LastAttack == null)
                        {
                            currentState = State.Idle;
                        }
                        break;
                }
                currentState = value;
            }
        }
    }
    [SerializeField]
    protected WeaponData weaponData;
    protected WeaponDamageComponent damageComponent;
    protected WeaponBlockComponent blockComponent;
    protected AnimancerComponent animancerComponent;
    protected StaminaComponent staminaComponent;
    public UnityEvent ChangedState { get; set; }
    public StaminaComponent StaminaComp
    {
        get
        {
            return staminaComponent;
        }
        set
        {
            staminaComponent = value;
            staminaComponent.Reset();
            blockComponent.StaminaComp = staminaComponent;
        }
    }
    protected Dictionary<Attack.Type, Attack> attacks;
    public Attack LastAttack
    {
        get
        {
            return damageComponent.CurrentAttack;
        }
        protected set
        {
            if (LastAttack != value)
            {
                LastAttack = damageComponent.CurrentAttack = value;
            }
        }
    }
    public float Reach
    {
        get
        {
            return weaponData.Reach;
        }
    }
    protected bool usedAlt;
    protected void Awake()
    {
        //get all the necessary components
        {
            animancerComponent = GetComponent<AnimancerComponent>();
            damageComponent = GetComponent<WeaponDamageComponent>();
            blockComponent = GetComponent<WeaponBlockComponent>();
        }

        //internalize animations in a dictionary for ease of access
        {
            attacks = new();
            foreach (Attack _atk in damageComponent.Attacks)
            {
                attacks.Add(_atk.AttackType, _atk);
            }
        }

        ChangedState = new();
    }
    protected void OnEnable()
    {
        ReturnToIdle();
    }
    public BlockResult Block(DmgInfo _dmgInfo)
    {
        switch (currentState)
        {
            case State.Blocking:
                var result = blockComponent.Block(_dmgInfo);
                if (result == BlockResult.Success)
                {
                    staminaComponent.DrainStamina(_dmgInfo.Attack.StaminaDamage);
                    if (_dmgInfo.Attack.AttackStrength == Attack.Strength.Heavy && !weaponData.Shield)
                    {
                        return BlockResult.Partial;
                    }
                }
                return result;

            case State.Windup:
                if (_dmgInfo.Attack.AttackType == LastAttack.AttackType)
                {
                    var res = blockComponent.Block(_dmgInfo);
                    if (res == BlockResult.Success)
                    {
                        return BlockResult.Counter;
                    }
                }
                return BlockResult.Failure;

            default:
                return BlockResult.Failure;
        }
    }
    public void Block(bool block)
    {
        switch (currentState)
        {
            case State.Idle:
                if (block)
                {
                    animancerComponent.Play(weaponData.BlockAnim, .25f);
                }
                break;
            case State.Blocking:
                if (!block)
                {
                    ReturnToIdle();
                }
                break;
        }
    }
    public void PerformAttack(Attack.Type _type, bool _alt = false)
    {
        usedAlt = _alt;
        switch (currentState)
        {
            case State.Idle:
                PerformAttack(attacks[_type], _alt);
                break;
            case State.Blocking:
                PerformAttack(attacks[_type], _alt);
                Block(false);
                break;
            case State.Recovery:
                if (LastAttack.AttackType == _type)
                {
                    //perform a combo, WIP
                    PerformAttack(attacks[_type], !usedAlt);
                }
                break;
        }
    }
    protected AnimancerState PerformAttack(Attack _attack, bool _alt)
    {
        usedAlt = _alt;
        LastAttack = _attack;
        AnimancerState state;
        if (_alt)
        {
            state = animancerComponent.Play(_attack.Alternate, .25f);
        }
        else
        {
            state = animancerComponent.Play(_attack.Regular, .25f);
        }
        state.Events(this).OnEnd ??= ReturnToIdle;
        //perform lunge
        return state;
    }
    protected void ReturnToIdle()
    {
        animancerComponent.Play(weaponData.IdleAnim);
    }
}