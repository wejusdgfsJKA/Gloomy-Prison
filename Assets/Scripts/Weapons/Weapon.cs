using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//a partial block refers to blocking a heavy attack without a shield
//a perfect block refers to a timed block
public enum BlockResult { Perfect, Success, Partial, Failed }
public class Weapon : MonoBehaviour
{
    //main weapon class which can manage multiple hitboxes
    //also manages blocking attacks
    [SerializeField]
    protected ImmutableWeaponData weaponData;
    public HashSet<Transform> hits { get; protected set; } = new HashSet<Transform>();
    protected Coroutine coroutine;
    protected WaitForSeconds wait;
    [SerializeField]
    protected Hurtbox[] hurtboxes = null;
    protected float StartedBlocking;
    [SerializeField]
    protected Transform BlockPoint, Center;
    protected void Awake()
    {
        //set up all the information of the dmgInfo class which will
        //not change for this object
        weaponData.dmgInfo.owner = transform.root;
        //set up all the hurtboxes, which is a PITA to do manually; WIP
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].Init(weaponData.mask, weaponData.MaxHurtboxEntities);
        }
        //cache the WaitForSeconds
        wait = new WaitForSeconds(weaponData.CheckInterval);
        StartedBlocking = -1;
    }
    public Hurtbox[] getHurtboxes()
    {
        return hurtboxes;
    }
    public AttackType GetCurrentAttackType()
    {
        return weaponData.dmgInfo.attackType;
    }
    public void EnableCollision()
    {
        StopBlocking();
        //the weapon is enabled, so we begin checking for collisions
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].ResetPreviouses();
        }
        coroutine = StartCoroutine(collisionCheck());
    }
    public void DisableCollision()
    {
        //we disable the weapon hurtboxes; this means just stopping the coroutine
        //and clearing the set of hits
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        hits.Clear();
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
                Transform parent = hurtbox.hits[j].transform.root;
                if (!hits.Contains(parent))
                {
                    //we have not hit this object before during this swing;this is to
                    //account for objects with multiple colliders, otherwise they
                    //might be hit multiple times during a swing
                    //we need the contact point to determine if a block is successful or not
                    weaponData.dmgInfo.ContactPoint = CalculateContactPoint(hurtbox, hurtbox.hits[j]);
                    DealDamage(parent);
                    hits.Add(parent);
                }
            }
        }
    }
    protected Vector3 CalculateContactPoint(Hurtbox hurtbox, Collider hit)
    {
        Vector3 point = hurtbox.GetCollider().ClosestPointOnBounds(hit.transform.position);
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
        EntityManager.instance.SendAttack(target, weaponData.dmgInfo);
    }
    public void StartBlocking()
    {
        DisableCollision();
        StartedBlocking = Time.time;
    }
    public void StopBlocking()
    {
        StartedBlocking = -1;
    }
    public BlockResult Block(DmgInfo dmgInfo)
    {
        if (StartedBlocking > 0)
        {
            if (Time.time <= StartedBlocking + Settings.instance.TimedBlockWindow)
            {
                //a timed block was executed
                return SuccessfullBlock(dmgInfo, true);
            }
            //we need to calculate the angle at which the attack hit us
            float strikeangle = Mathf.Acos(Vector3.Dot(BlockPoint.forward,
                 (dmgInfo.ContactPoint - BlockPoint.position).normalized)) * Mathf.Rad2Deg;
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
    public BlockResult SuccessfullBlock(DmgInfo dmgInfo, bool timed = false)
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
            if (!weaponData.shield)
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
    //the following are just getters for the editor and other scripts
    public float GetStrikeBlockAngle()
    {
        return weaponData.StrikeBlockAngle;
    }
    public float GetThrustBlockAngle()
    {
        return weaponData.ThrustBlockAngle;
    }
    public float GetReach()
    {
        return weaponData.reach;
    }
    public bool GetShieldBool()
    {
        return weaponData.shield;
    }
    public Transform GetBlockPoint()
    {
        return BlockPoint;
    }
    public Transform GetCenter()
    {
        return Center;
    }
}