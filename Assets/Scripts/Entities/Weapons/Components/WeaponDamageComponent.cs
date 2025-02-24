using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages the damage part of a weapon.
/// </summary>
public class WeaponDamageComponent : MonoBehaviour
{
    [SerializeField] protected WeaponDamageData data;
    /// <summary>
    /// All of the attacks the weapon can perform.
    /// </summary>
    public Attack[] Attacks
    {
        get
        {
            return data.Attacks;
        }
    }
    /// <summary>
    /// All the objects the weapon has hit, only registers 
    /// once for any given root transform.
    /// </summary>
    public HashSet<Transform> Hits { get; protected set; } = new();
    protected Coroutine coroutine;
    protected WaitForSeconds wait;
    /// <summary>
    /// All the hurtboxes of the weapon.
    /// </summary>
    [SerializeField]
    protected Hurtbox[] hurtboxes = null;
    /// <summary>
    /// All of this weapon's hurtboxes.
    /// </summary>
    public Hurtbox[] Hurtboxes
    {
        get
        {
            return hurtboxes;
        }
    }
    /// <summary>
    /// The damage package the weapon will use.
    /// </summary>
    protected DmgInfo dmgInfo;
    /// <summary>
    /// The attack we are currently using.
    /// </summary>

    [SerializeField] protected Attack currentAttack;
    /// <summary>
    /// The attack we are currently using.
    /// </summary>
    public Attack CurrentAttack
    {
        get
        {
            return currentAttack;
        }
        set
        {
            currentAttack = value;
        }
    }
    protected void Awake()
    {
        //set up all the information of the dmgInfo class which will
        //not change for this object
        dmgInfo = new DmgInfo(transform.root);
        //set up all the hurtboxes, which is a PITA to do manually; WIP
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].Init(data.Mask, data.MaxHurtboxEntities);
        }
        //cache the WaitForSeconds
        wait = new WaitForSeconds(data.CheckInterval);
    }
    /// <summary>
    /// Enable the weapon, begin checking for collisions.
    /// </summary>
    public void EnterDamageState()
    {
        dmgInfo.Attack = CurrentAttack;
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            //reset all hurtboxes
            hurtboxes[i].ResetPreviouses();
        }
        coroutine = StartCoroutine(collisionCheck());
    }
    /// <summary>
    /// The weapon is disabled. Stop the collision check
    /// coroutine and clear the Hits HashMap.
    /// </summary>
    public void ExitDamageState()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        if (Hits.Count > 0)
        {
            Hits.Clear();
        }
    }
    /// <summary>
    /// While active, check all hurtboxes.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator collisionCheck()
    {
        while (true)
        {
            //check all hurtboxes
            checkHurtboxes();
            yield return wait;
        }
    }
    /// <summary>
    /// Check all hurtboxes. If we hit a child of object A, 
    /// we will ignore any other hits on other children of 
    /// object A or on A itself.
    /// </summary>
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
    /// <summary>
    /// Attempt to estimate the contact point with an entity.
    /// </summary>
    /// <param name="hurtbox">The hurtbox which scored the hit.</param>
    /// <param name="hit">The collider we hit.</param>
    /// <returns></returns>
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
    /// <summary>
    /// Send the damage package to the target;
    /// </summary>
    /// <param name="target">The target we are trying to damage.</param>
    protected void DealDamage(Transform target)
    {
        EntityManager.Instance.SendAttack(target, dmgInfo);
    }
}