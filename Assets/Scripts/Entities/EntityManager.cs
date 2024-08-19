using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance { get; private set; }
    //we handle pooling and damage here
    [SerializeField]
    protected Dictionary<string, EntityBase> roster;
    public Dictionary<Transform, EntityBase> entities { get; private set; }
    public Dictionary<string, Queue<EntityBase>> pool { get; private set; }
    public bool b1, b2;
    public Transform p1, p2;
    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void OnEnable()
    {
        roster = new();
        entities = new();
        pool = new();
    }
    private void Update()
    {
        if (b1)
        {
            b1 = false;
            try
            {
                Spawn("Dummy Player", p1.position, p1.rotation);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        if (b2)
        {
            b2 = false;
            try
            {
                Spawn("Dummy Monster", p2.position, p2.rotation);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
    protected void Spawn(string entityName, Vector3 position, Quaternion rotation)
    {
        try
        {
            if (pool[entityName].Count > 0)
            {
                //we have inactive entities we can reactivate
                EntityBase entity = pool[entityName].Dequeue();
                entity.transform.position = position;
                entity.transform.rotation = rotation;
                entity.gameObject.SetActive(true);
            }
            else
            {
                //the pool was empty
                CreateNew(entityName, position, rotation);
            }
        }
        catch (KeyNotFoundException)
        {
            //we had no available pool
            pool.Add(entityName, new Queue<EntityBase>());
            CreateNew(entityName, position, rotation);
        }
    }
    protected void CreateNew(string EntityName, Vector3 position, Quaternion rotation)
    {
        try
        {
            //we instantiate a new prefab
            EntityBase entity = Instantiate(roster[EntityName], position, rotation);
            //register the script in the transform dictionary, to allow damage to pass to it
            //via its transform
            entities.Add(entity.transform, entity);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void DealDamage(Transform entity, DmgInfo dmgInfo)
    {
        //send the entity the damage package
        try
        {
            entities[entity].ReceiveAttack(dmgInfo);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }
    public void Dead(EntityBase entity)
    {
        //this entity just died
        try
        {
            pool[entity.entityData.Name].Enqueue(entity);
        }
        catch (KeyNotFoundException)
        {
            pool.Add(entity.entityData.Name, new Queue<EntityBase>());
            pool[entity.entityData.Name].Enqueue(entity);
        }
    }
    public void AddToRoster(EntityData entityData)
    {
        //add an entity to the roster
        try
        {
            roster.Add(entityData.Name, entityData.prefab);
            //Debug.Log("Added " + entityData.Name + " to roster.");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        try
        {
            pool.Add(entityData.Name, new Queue<EntityBase>());
            //Debug.Log("Added " + entityData.Name + " to roster.");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void SpawnAndKill(EntityBase entity)
    {
        //create an entity and immediately deactivate it; used for generating a pool
        //at the start of the level/mission/run w/e
        //we instantiate a new prefab
        Instantiate(entity);
        //register the script in the transform dictionary, to allow damage to pass to it
        //via its transform
        entities.Add(entity.transform, entity);
        entity.gameObject.SetActive(false);
    }
}
