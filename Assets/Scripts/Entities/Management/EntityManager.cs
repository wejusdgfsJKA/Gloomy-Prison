using System.Collections.Generic;
using UnityEngine;

//this script will handle pooling, spawning, taking damage etc.
public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; protected set; }
    [SerializeField]
    protected Dictionary<string, EntityBase> roster;
    public Dictionary<Transform, EntityBase> Entities { get; private set; }
    protected Dictionary<string, Queue<EntityBase>> pool;
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        roster = new();
        Entities = new();
        pool = new();
    }
    public void AddToRoster(EntityData _entityData)
    {
        //add an entity to the roster
        try
        {
            roster.Add(_entityData.Name, _entityData.Prefab);
            //Debug.Log("Added " + entityData.Name + " to roster.");
        }
        catch (System.Exception _e)
        {
            Debug.Log(_e.Message);
        }
        try
        {
            pool.Add(_entityData.Name, new Queue<EntityBase>());
            //Debug.Log("Added " + entityData.Name + " to roster.");
        }
        catch (System.Exception _e)
        {
            Debug.Log(_e.Message);
        }
    }
    public void SpawnAndKill(EntityBase _entity)
    {
        //create an entity and immediately deactivate it; used for generating a pool
        //at the start of the level
        Instantiate(_entity);
        //register the script in the transform dictionary, to allow damage to
        //pass to it via its transform
        Entities.Add(_entity.transform, _entity);
        _entity.gameObject.SetActive(false);
    }
    protected void CreateNew(string _entityname, Vector3 _position, Quaternion _rotation)
    {
        try
        {
            //we instantiate a new prefab
            EntityBase entity = Instantiate(roster[_entityname], _position, _rotation);
            //register the script in the transform dictionary, to allow damage to pass to it
            //via its transform
            Entities.Add(entity.transform, entity);
        }
        catch (System.Exception _e)
        {
            Debug.Log(_e.Message);
        }
    }
    protected void Spawn(string _entityName, Vector3 _position, Quaternion _rotation)
    {
        try
        {
            if (pool[_entityName].Count > 0)
            {
                //we have inactive entities we can reactivate
                EntityBase _entity = pool[_entityName].Dequeue();
                _entity.transform.position = _position;
                _entity.transform.rotation = _rotation;
                _entity.gameObject.SetActive(true);
            }
            else
            {
                //the pool was empty
                CreateNew(_entityName, _position, _rotation);
            }
        }
        catch (KeyNotFoundException)
        {
            //we had no available pool
            pool.Add(_entityName, new Queue<EntityBase>());
            CreateNew(_entityName, _position, _rotation);
        }
    }
    public void Dead(EntityBase _entity)
    {
        //this entity just died
        try
        {
            pool[_entity.Name].Enqueue(_entity);
        }
        catch (KeyNotFoundException)
        {
            pool.Add(_entity.Name, new Queue<EntityBase>());
            pool[_entity.Name].Enqueue(_entity);
        }
    }
    public void SendAttack(Transform _entity, DmgInfo _dmgInfo)
    {
        //send the entity the damage package
        try
        {
            Entities[_entity].ReceiveAttack(_dmgInfo);
        }
        catch (System.Exception _e)
        {
            Debug.LogException(_e);
        }
    }
    public void SendAttackResult(BlockResult _blockResult, DmgInfo _dmgInfo)
    {
        //send the damage dealer information about what happened to the defender
        try
        {
            Entities[_dmgInfo.Owner].ReceiveAttackResult(_blockResult, _dmgInfo);
        }
        catch (System.Exception _e)
        {
            Debug.LogException(_e);
        }
    }
}
