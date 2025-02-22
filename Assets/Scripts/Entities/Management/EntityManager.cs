using System.Collections.Generic;
using UnityEngine;

//this script will handle taking damage etc.
public class EntityManager : MonoBehaviour
{
    /// <summary>
    /// The instance of the manager, only one can exist at any given time.
    /// </summary>
    public static EntityManager Instance { get; protected set; }
    /// <summary>
    /// The roster of entities that can be spawned.
    /// </summary>
    protected Dictionary<int, EntityBase> roster = new();
    /// <summary>
    /// The list of active entities.
    /// </summary>
    protected Dictionary<Transform, EntityBase> entities = new();
    /// <summary>
    /// The pool of available entities. The key is each entity's internal ID.
    /// </summary>
    protected Dictionary<int, Queue<EntityBase>> pool = new();
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    /// <summary>
    /// Adds a new entity to the manager's roster of possible entitys to spawn.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>True if the entity was added succesfully.</returns>
    public bool AddToRoster(EntityBase entity)
    {
        if (roster.TryAdd(entity.ID, entity))
        {
            pool.Add(entity.ID, new Queue<EntityBase>());
            return true;
        }
        return false;
    }
    /// <summary>
    /// Get an entity from the roster.
    /// </summary>
    /// <param name="EntityID">The internal ID of the entity.</param>
    /// <returns>The entity if it is found in the roster, or null otherwise.</returns>
    public EntityBase GetFromRoster(int entityID)
    {
        EntityBase entity;
        if (roster.TryGetValue(entityID, out entity))
        {
            return entity;
        }
        return null;
    }
    /// <summary>
    /// Adds the entity to the manager's list of active entities, so it can be detected and receive attacks.
    /// </summary>
    /// <param name="Entity">The entity to be added.</param>
    public void RegisterEntity(EntityBase entity)
    {
        entities.Add(entity.transform, entity);
    }
    /// <summary>
    /// Get an entity from the pool of existing entities.
    /// </summary>
    /// <param name="entityID">Internal ID of the entity.</param>
    /// <param name="createPool">If true, create a new pool if no existing pool is found.</param>
    /// <returns>An entity instance if a valid one was found, null if the respective pool was empty or didn't exist.</returns>
    public EntityBase GetFromPool(int entityID, bool createPool)
    {
        Queue<EntityBase> queue;
        if (pool.TryGetValue(entityID, out queue))
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }
            return null;
        }
        if (createPool)
        {
            pool.Add(entityID, new Queue<EntityBase>());
        }
        return null;
    }
    /// <summary>
    /// An entity has been destroyed.
    /// </summary>
    /// <param name="entity">The EntityBase instance that was destroyed.</param>
    public void Dead(EntityBase entity)
    {
        if (entities.Remove(entity.transform))
        {
            pool[entity.ID].Enqueue(entity);
        }
    }
    /// <summary>
    /// Send a target the damage package.
    /// </summary>
    /// <param name="target"> The object being attacked. </param>
    /// <param name="dmgInfo"> The damage package. </param>
    /// <returns></returns>
    public bool SendAttack(Transform target, DmgInfo dmgInfo)
    {
        //send the entity the damage package
        EntityBase entity;
        if (entities.TryGetValue(target, out entity))
        {
            entity.ReceiveAttack(dmgInfo);
            return true;
        }
        return false;
    }
}
