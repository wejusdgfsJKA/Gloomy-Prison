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
    protected State currentState;
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
                switch (value)
                {
                    case State.Idle:
                        usedAlt = false;
                        feinted = false;
                        damageComponent.ExitDamageState();
                        break;
                    case State.Release:
                        feinted = false;
                        damageComponent.EnterDamageState();
                        break;
                    case State.Recovery:
                        feinted = false;
                        damageComponent.ExitDamageState();
                        break;
                    case State.Blocking:
                        usedAlt = false;
                        feinted = false;
                        damageComponent.ExitDamageState();
                        break;
                }
                currentState = value;
            }
        }
    }
    protected WeaponDamageComponent damageComponent;
    protected WeaponBlockComponent blockComponent;
    protected AnimancerComponent AnimComp;
    protected StaminaComponent staminaComponent;
    [SerializeField] protected AnimationClip IdleAnim;
    public StaminaComponent StaminaComp
    {
        get
        {
            return staminaComponent;
        }
    }
    protected Dictionary<Attack.Type, Attack> attacks;
    [SerializeField]
    protected Attack lastAttack;
    protected bool usedAlt, feinted;
    protected void Awake()
    {
        //get all the necessary components
        {
            AnimComp = GetComponent<AnimancerComponent>();
            damageComponent = GetComponent<WeaponDamageComponent>();
            blockComponent = GetComponent<WeaponBlockComponent>();
            staminaComponent = GetComponent<StaminaComponent>();
        }
        //internalize animations in a dictionary for ease of access
        {
            attacks = new();
            foreach (Attack _atk in damageComponent.Attacks)
            {
                _atk.Regular.Events.OnEnd = ReturnToIdle;
                _atk.Alternate.Events.OnEnd = ReturnToIdle;
                attacks.Add(_atk.AttackType, _atk);
            }
        }
    }
    protected void OnEnable()
    {
        usedAlt = false;
        feinted = false;
    }
    public BlockResult Block(DmgInfo _dmgInfo)
    {
        return blockComponent.Block(_dmgInfo);
    }
    public void PerformAttack(Attack.Type _type, bool _alt = false)
    {
        switch (currentState)
        {
            case State.Idle:
                PerformAttack(attacks[_type], _alt);
                break;
            case State.Windup:
                if (_type != lastAttack.AttackType && !feinted && lastAttack.Feintable)
                {
                    //perform a feint, WIP
                    feinted = true;
                    PerformAttack(attacks[_type], _alt);
                }
                break;
            case State.Recovery:
                if (lastAttack.AttackType == _type)
                {
                    //perform a combo, WIP
                    PerformAttack(attacks[_type], !usedAlt);
                }
                else
                {
                    //we are changing our attack, we need to buffer the input
                }
                break;
        }
    }
    protected AnimancerState PerformAttack(Attack _attack, bool _alt)
    {
        usedAlt = _alt;
        lastAttack = _attack;
        if (_alt)
        {
            return AnimComp.Play(_attack.Alternate);
        }
        else
        {
            return AnimComp.Play(_attack.Regular);
        }
    }
    protected void ReturnToIdle()
    {
        AnimComp.Play(IdleAnim);
    }
}