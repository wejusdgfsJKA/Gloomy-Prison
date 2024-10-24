using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponDamageComponent : MonoBehaviour
{
    //manages the damage part of a weapon
    [SerializeField] protected WeaponDamageData data;
    public Attack[] Attacks
    {
        get
        {
            return data.Attacks;
        }
    }
    public HashSet<Transform> Hits { get; protected set; } = new();
    protected Coroutine coroutine;
    protected WaitForSeconds wait;
    [SerializeField]
    protected Hurtbox[] hurtboxes = null;
    protected DmgInfo dmgInfo;

    [SerializeField] protected Attack currentAttack;
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
    public void EnterDamageState()
    {
        dmgInfo.Attack = CurrentAttack;
        //the weapon is enabled, so we begin checking for collisions
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].ResetPreviouses();
        }
        coroutine = StartCoroutine(collisionCheck());
    }
    public void ExitDamageState()
    {
        //we disable the weapon hurtboxes; this means just
        //stopping the coroutine and clearing the set of hits
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        if (Hits.Count > 0)
        {
            Hits.Clear();
        }
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
        EntityManager.Instance.SendAttack(target, dmgInfo);
    }
    public Hurtbox[] GetHurtboxes()
    {
        return hurtboxes;
    }
}