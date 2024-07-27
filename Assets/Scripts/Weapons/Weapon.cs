using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    //main weapon class which can manage multiple hitboxes
    [SerializeField]
    protected ImmutableWeaponData weaponData;
    public HashSet<Transform> hits { get; protected set; } = new HashSet<Transform>();
    protected Coroutine coroutine;
    protected WaitForSeconds wait;
    [SerializeField]
    protected Hurtbox[] hurtboxes = null;
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
        //will come in handy for object pooling
        hits.Clear();
        //EnableWeapon();
    }
    public void EnableWeapon1()
    {
        //the weapon is enabled, so we begin checking for collisions
        coroutine = StartCoroutine(collisionCheck());
    }
    public void DisableWeapon1()
    {
        //we disable the weapon hurtboxes; in this case we just stop the coroutine
        //and clear the set of hits
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        hits.Clear();
    }
    protected void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
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
        for (int i = 0; i < hurts.Length; i++)
        {
            //cache the current hurtbox and find out its type with dynamic casting
            if (hurts[i].points.Length == 2)
            {
                //we have a capsule-shaped hurtbox
                DrawCapsuleHurtbox(hurts[i]);
            }
            else
            {
                //we have a box-shaped hurtbox
                DrawBoxHurtbox(hurts[i]);
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