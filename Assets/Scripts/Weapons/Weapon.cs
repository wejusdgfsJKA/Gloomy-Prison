using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField]
    MeshRenderer meshRenderer;
    protected float StartedBlocking;
    public bool blocking;
    [SerializeField]
    protected Transform BlockPoint;
    protected void Awake()
    {
        //set up all the information of the dmgInfo class which will
        //not change for this object
        weaponData.dmgInfo.owner = transform.root;
        //set up all the hurtboxes, which is a PITA to do manually; WIP
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].mask = weaponData.mask;
            hurtboxes[i].maxEntities = weaponData.MaxHurtboxEntities;
            hurtboxes[i].hits = new Collider[hurtboxes[i].maxEntities];
        }
        //cache the WaitForSeconds
        wait = new WaitForSeconds(weaponData.CheckInterval);
    }
    public Hurtbox[] getHurtboxes()
    {
        return hurtboxes;
    }
    protected void OnEnable()
    {
        EnableWeapon();
    }
    public void EnableWeapon()
    {
        //the weapon is enabled, so we begin checking for collisions
        coroutine = StartCoroutine(collisionCheck());
        meshRenderer.material.color = Color.red;
    }
    public void DisableWeapon()
    {
        //we disable the weapon hurtboxes; in this case we just stop the coroutine
        //and clear the set of hits
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        hits.Clear();
        meshRenderer.material.color = Color.green;
    }
    protected void OnDisable()
    {
        DisableWeapon();
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
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            int count = hurtboxes[i].CheckVolume();
            //we need to use this count because if we use array.length it will go over
            //all the NULL elements as well
            for (int j = 0; j < count; j++)
            {
                Transform parent = hurtboxes[i].hits[j].transform.root;
                if (!hits.Contains(parent))
                {
                    //we have not hit this object before during this swing;this is to
                    //account for objects with multiple colliders, otherwise they
                    //might be hit multiple times during a swing
                    dealDamage(parent);
                    hits.Add(parent);
                }
            }
        }
    }
    protected void dealDamage(Transform target)
    {
        //Debug.Log(target);
        EntityManager.instance.DealDamage(target, weaponData.dmgInfo);
    }
    public void StartBlocking()
    {
        blocking = true;
        DisableWeapon();
        Debug.Log("Blocking");
        StartedBlocking = Time.time;
    }
    public void StopBlocking()
    {
        blocking = false;
        Debug.Log("Not blocking");
    }
    public BlockResult Block(DmgInfo dmgInfo)
    {
        if (blocking)
        {
            if (Time.time <= StartedBlocking + Settings.instance.TimedBlockWindow)
            {
                //a timed block was executed
                return SuccessfullBlock(dmgInfo, true);
            }
            //we need to calculate the angle at which the attack hit us
            float strikeangle = Mathf.Acos(Vector3.Dot(BlockPoint.forward,
                BlockPoint.position - dmgInfo.ContactPoint)) * Mathf.Rad2Deg;
            if (dmgInfo.attackType == DmgInfo.AttackType.Strike)
            {
                if (strikeangle <= weaponData.StrikeBlockAngle)
                {
                    //we successfully blocked a strike
                    return SuccessfullBlock(dmgInfo);
                }
            }
            else
            {
                if (dmgInfo.attackType == DmgInfo.AttackType.Thrust)
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
        if (dmgInfo.strength == DmgInfo.AttackStrength.Regular)
        {
            //we successfully blocked a regular attack
            if (timed)
            {
                //maybe we want to stagger the opponent?
                return BlockResult.Perfect;
            }
            return BlockResult.Success;
        }
        if (dmgInfo.strength == DmgInfo.AttackStrength.Heavy)
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
}
#if UNITY_EDITOR
[CustomEditor(typeof(Weapon))]
public class WeaponDebug : Editor
{
    private void OnSceneGUI()
    {
        //draw the hurtboxes
        Weapon weapon = (Weapon)target;
        var hurts = weapon.getHurtboxes();
        Handles.color = Color.yellow;
        if (hurts != null)
        {
            for (int i = 0; i < hurts.Length; i++)
            {
                //cache the current hurtbox and find out its type with dynamic casting
                if (hurts[i].points.Length == 2 && hurts[i].floats.Length == 1)
                {
                    //we have a capsule-shaped hurtbox
                    DrawCapsuleHurtbox(hurts[i]);
                }
                else if (hurts[i].points.Length == 1 && hurts[i].floats.Length == 3)
                {
                    //we have a box-shaped hurtbox
                    DrawBoxHurtbox(hurts[i]);
                }
            }
        }
    }
    void DrawCapsuleHurtbox(Hurtbox box)
    {
        Handles.DrawWireArc(box.points[0].position, box.points[0].up,
            box.points[0].forward, 360, box.floats[0]);
        Handles.DrawWireArc(box.points[0].position, box.points[0].forward,
            box.points[0].right, 80, box.floats[0]);
        Handles.DrawWireArc(box.points[0].position, box.points[0].right,
            box.points[0].forward, -180, box.floats[0]);
        //draw the body of the capsule
        Handles.DrawLine(box.points[0].position + box.points[0].right * box.floats[0],
            box.points[1].position + box.points[1].right * box.floats[0]);
        Handles.DrawLine(box.points[0].position - box.points[0].right * box.floats[0],
            box.points[1].position - box.points[1].right * box.floats[0]);
        Handles.DrawLine(box.points[0].position + box.points[0].forward * box.floats[0],
            box.points[1].position + box.points[1].forward * box.floats[0]);
        Handles.DrawLine(box.points[0].position - box.points[0].forward * box.floats[0],
            box.points[1].position - box.points[1].forward * box.floats[0]);
        //draw the bottom of the capsule
        Handles.DrawWireArc(box.points[1].position, box.points[1].up, box.points[1].forward,
            360, box.floats[0]);
        Handles.DrawWireArc(box.points[1].position, box.points[1].forward, box.points[1].right,
            -180, box.floats[0]);
        Handles.DrawWireArc(box.points[1].position, box.points[1].right, box.points[1].forward,
            180, box.floats[0]);
    }
    void DrawBoxHurtbox(Hurtbox box)
    {
        //draw the top of the box
        {
            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            box.floats[1] + box.points[0].forward * box.floats[2],
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            box.floats[1] + box.points[0].forward * box.floats[2]);

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            box.floats[1] + box.points[0].forward * box.floats[2],
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            box.floats[1] + box.points[0].forward * (-box.floats[2]));

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            box.floats[1] + box.points[0].forward * box.floats[2],
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            box.floats[1] + box.points[0].forward * (-box.floats[2]));

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            box.floats[1] + box.points[0].forward * (-box.floats[2]),
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            box.floats[1] + box.points[0].forward * (-box.floats[2]));
        }
        //draw the body of the box
        {
            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            box.floats[1] + box.points[0].forward * box.floats[2],
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * box.floats[2]);

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            box.floats[1] + box.points[0].forward * (-box.floats[2]),
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * (-box.floats[2]));

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            box.floats[1] + box.points[0].forward * (-box.floats[2]),
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * (-box.floats[2]));

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            box.floats[1] + box.points[0].forward * (-box.floats[2]),
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * (-box.floats[2]));
        }
        //draw the bottom of the box
        {
            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * box.floats[2],
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * box.floats[2]);

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * box.floats[2],
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * (-box.floats[2]));

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * box.floats[2],
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * (-box.floats[2]));

            Handles.DrawLine(
            box.points[0].position +
            box.points[0].right * box.floats[0] + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * (-box.floats[2]),
            box.points[0].position +
            box.points[0].right * (-box.floats[0]) + box.points[0].up *
            (-box.floats[1]) + box.points[0].forward * (-box.floats[2]));
        }
    }
}
#endif