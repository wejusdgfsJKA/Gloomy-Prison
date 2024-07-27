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
    public bool b;
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
        if (b)
        {
            b = false;
            Spawn("Detectable", new Vector3(0, 1, 10), Quaternion.identity);
        }
    }
    protected void Spawn(string entityName, Vector3 position, Quaternion rotation)
    {
        if (pool.ContainsKey(entityName))
        {
            if (pool[entityName].Count > 0)
            {
                //we have inactive entities we can reactivate
                EntityBase entity = pool[entityName].Dequeue();
                entity.transform.position = position;
                entity.transform.rotation = rotation;
                entity.gameObject.SetActive(true);
                return;
            }
        }
        else
        {
            //we had no available pool
            pool.Add(entityName, new Queue<EntityBase>());
        }
        CreateNew(entityName, position, rotation);
    }
    protected void CreateNew(string EntityName, Vector3 position, Quaternion rotation)
    {
        //we instantiate a new prefab
        EntityBase entity = Instantiate(roster[EntityName], position, rotation);
        //register the script in the transform dictionary, to allow damage to pass to it
        //via its transform
        entities[entity.transform] = entity;
    }
    public void DealDamage(Transform entity, DmgInfo dmgInfo)
    {
        //send the entity the damage package
        if (entities.ContainsKey(entity))
        {
            entities[entity].ReceiveAttack(dmgInfo);
        }
    }
    public void Dead(EntityBase entity)
    {
        //this entity just died
        pool[entity.entityData.Name].Enqueue(entity);
    }
    public void AddToRoster(EntityBase entity, string Name)
    {
        //add an entity to the roster
        if (!roster.
            ContainsKey(
            Name))
        {
            roster.Add(Name, entity);
        }
        if (!pool.ContainsKey(Name))
        {
            pool.Add(Name, new Queue<EntityBase>());
        }
    }
    public void SpawnAndKill(AIEntity entity)
    {
        //create an entity and immediately deactivate it; used for generating a pool
        //at the start of the level/mission/run w/e
        //we instantiate a new prefab
        Instantiate(entity);
        //register the script in the transform dictionary, to allow damage to pass to it
        //via its transform
        entities[entity.transform] = entity;
        entity.gameObject.SetActive(false);
    }
}
