using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//a partial block refers to blocking a heavy attack without a shield
//a perfect block refers to a timed block
public enum BlockResult { Perfect, Success, Partial, Failed }
public enum WeaponState { Idle, WindingUp, Attacking, Pushing, Blocking }
public class Weapon : MonoBehaviour
{
    //main weapon class which can manage multiple hitboxes
    //also manages blocking
    [SerializeField]
    protected WeaponData weaponData;
    [SerializeField] protected Receiver windupReceiver;
    [SerializeField] protected Receiver attackReceiver;
    [SerializeField] protected Receiver idleReceiver;
    [SerializeField] protected Receiver blockReceiver;
    [SerializeField] protected Receiver pushReceiver;
    public HashSet<Transform> Hits { get; protected set; }
    protected Coroutine coroutine;
    protected WaitForSeconds wait;
    [SerializeField]
    protected Hurtbox[] hurtboxes = null;
    protected float startedBlocking;
    [SerializeField]
    protected Transform blockPoint, centerPoint;//the center will be used
    //for the AI to know in which direction to block
    public WeaponAnimHandler AnimHandler { get; protected set; }
    public bool IsShield
    { get { return weaponData.Shield; } }
    public float Reach { get { return weaponData.Reach; } }
    protected DmgInfo dmgInfo;
    public int CurrentAttack { get; set; }
    public WeaponState CurrentState
    {
        get; protected set;
    }
    protected void Awake()
    {
        //set up all the information of the dmgInfo class which will
        //not change for this object
        dmgInfo = new DmgInfo(transform.root);
        CurrentAttack = 0;
        //set up all the hurtboxes, which is a PITA to do manually; WIP
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].Init(weaponData.Mask, weaponData.MaxHurtboxEntities);
        }
        //cache the WaitForSeconds
        wait = new WaitForSeconds(weaponData.CheckInterval);
        startedBlocking = -1;
        AnimHandler = GetComponentInChildren<WeaponAnimHandler>();
        Hits = new();
    }
    protected void Start()
    {
        SetupReceivers();
    }
    protected void SetupReceivers()
    {
        //assign all receivers their corresponding methods
        windupReceiver.AddAction(EnterWindupState);
        attackReceiver.AddAction(EnterDamageState);
        idleReceiver.AddAction(EnterIdleState);
        blockReceiver.AddAction(EnterBlockState);
        pushReceiver.AddAction(EnterPushingState);
    }
    public void EnterDamageState()
    {
        ExitBlockState();
        CurrentState = WeaponState.Attacking;
        dmgInfo.attack = weaponData.Attacks[CurrentAttack];
        //the weapon is enabled, so we begin checking for collisions
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].ResetPreviouses();
        }
        coroutine = StartCoroutine(collisionCheck());
    }
    public void ExitDamageState()
    {
        //we disable the weapon hurtboxes; this means just stopping the coroutine
        //and clearing the set of hits
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        Hits.Clear();
    }
    protected IEnumerator collisionCheck()
    {
        while (true)
        {
            //check all hurtboxes
            checkHurtboxes();
            yield return wait;
        }
    }
    protected void checkHurtboxes()
    {
        foreach (var hurtbox in hurtboxes)
        {
            hurtbox.AddPrevious();
            int count = hurtbox.CheckVolume();
            //we need to use this count because if we use array.length it will go over
            //all the NULL elements as well
            for (int j = 0; j < count; j++)
            {
                Transform parent = hurtbox.Hits[j].transform.root;
                if (!Hits.Contains(parent))
                {
                    //we have not hit this object before during this swing;this is to
                    //account for objects with multiple colliders, otherwise they
                    //might be hit multiple times during a swing
                    //we need the contact point to determine if a block is successful or not
                    dmgInfo.ContactPoint = CalculateContactPoint(hurtbox, hurtbox.Hits[j]);
                    DealDamage(parent);
                    Hits.Add(parent);
                }
            }
        }
    }
    protected Vector3 CalculateContactPoint(Hurtbox hurtbox, Collider hit)
    {
        Vector3 point = hurtbox.Collider.ClosestPointOnBounds(hit.transform.position);
        Vector3 prev = hurtbox.PreviousPositions.First.Value;
        foreach (var p in hurtbox.PreviousPositions)
        {
            if (prev != p)
            {
                //Debug.DrawRay(point, dir, Color.green, 10);
                point = hit.ClosestPoint(hit.ClosestPointOnBounds(point + prev - p));
                prev = p;
            }
        }
        point = hit.ClosestPoint(hit.ClosestPointOnBounds(point));
        //Debug.DrawLine(point, hit.transform.GetComponent<EntityBase>().GetBlockPoint().position, Color.red, 10);
        return point;
    }
    protected void DealDamage(Transform target)
    {
        //Debug.Log(target);
        EntityManager.Instance.SendAttack(target, dmgInfo);
    }
    //block-related functions
    protected void EnterBlockState()
    {
        ExitDamageState();
        startedBlocking = Time.time;
    }
    protected void ExitBlockState()
    {
        startedBlocking = -1;
    }
    public BlockResult Block(DmgInfo dmgInfo)
    {
        if (startedBlocking > 0)
        {
            if (Time.time <= startedBlocking + Settings.Instance.TimedBlockWindow)
            {
                //a timed block was executed
                return SuccessfullBlock(dmgInfo, true);
            }
            //we need to calculate the angle at which the attack hit us
            float strikeangle = Mathf.Acos(Vector3.Dot(blockPoint.forward,
                 (dmgInfo.ContactPoint - blockPoint.position).normalized)) * Mathf.Rad2Deg;
            //Debug.Log(strikeangle);
            if (dmgInfo.attackType == AttackType.Strike)
            {
                if (strikeangle <= weaponData.StrikeBlockAngle)
                {
                    //we successfully blocked a strike
                    return SuccessfullBlock(dmgInfo);
                }
            }
            else
            {
                if (dmgInfo.attackType == AttackType.Thrust)
                {
                    if (strikeangle <= weaponData.ThrustBlockAngle)
                    {
                        //we successfully blocked a thrust
                        return SuccessfullBlock(dmgInfo);
                    }
                }
            }
        }
        return BlockResult.Failed;
    }
    protected BlockResult SuccessfullBlock(DmgInfo dmgInfo, bool timed = false)
    {
        if (dmgInfo.strength == AttackStrength.Regular)
        {
            //we successfully blocked a regular attack
            if (timed)
            {
                //maybe we want to stagger the opponent?
                return BlockResult.Perfect;
            }
            return BlockResult.Success;
        }
        if (dmgInfo.strength == AttackStrength.Heavy)
        {
            //we need a shield to block this
            if (!weaponData.Shield)
            {
                //a heavy blow like this can only be blocked with a shield
                return BlockResult.Partial;
            }
            if (timed)
            {
                return BlockResult.Perfect;
            }
            return BlockResult.Success;
        }
        return BlockResult.Failed;
    }
    protected void EnterIdleState()
    {
        //we are now idle (or staggered)
        ExitBlockState();
        ExitDamageState();
        CurrentState = WeaponState.Idle;
    }
    protected void EnterWindupState()
    {
        CurrentState = WeaponState.WindingUp;
    }
    protected void EnterPushingState()
    {
        CurrentState = WeaponState.Pushing;
    }
}