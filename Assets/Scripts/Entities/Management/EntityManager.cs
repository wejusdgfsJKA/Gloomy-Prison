using System.Collections.Generic;
using UnityEngine;

//this script will handle taking damage etc.
public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; protected set; }
    public Dictionary<string, EntityBase> Roster { get; protected set; }
    public Dictionary<Transform, EntityBase> Entities { get; protected set; }
    protected Dictionary<string, Queue<EntityBase>> pool;
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    protected void OnEnable()
    {
        Roster = new();
        Entities = new();
        pool = new();
    }
    public void AddToRoster(EntityData _entityData)
    {
        //add an entity to the roster
        try
        {
            Roster.Add(_entityData.Name, _entityData.Prefab);
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
    public void RegisterEntity(EntityBase _entity)
    {
        Entities.Add(_entity.transform, _entity);
    }
    public void AddToPool(string _entity)
    {
        pool.Add(_entity, new Queue<EntityBase>());
    }
    public EntityBase GetFromPool(string _entity)
    {
        if (pool[_entity].Count > 0)
        {
            return pool[_entity].Dequeue();
        }
        return null;
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
