using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    /// <summary>
    /// Spawn a new entity.
    /// </summary>
    /// <param name="entityID">Internal ID of the entity.</param>
    /// <param name="position">Desired position.</param>
    /// <param name="rotation">Desired rotation.</param>
    public void Spawn(int entityID, Vector3 position, Quaternion rotation)
    {
        EntityBase entity = EntityManager.Instance.GetFromPool(entityID, true);
        if (entity != null)
        {
            //we have inactive entities we can reactivate
            entity.transform.position = position;
            entity.transform.rotation = rotation;
            entity.gameObject.SetActive(true);
            return;
        }
        else
        {
            //the pool was empty, so we must instantiate a new prefab
            EntityBase newEntity = Instantiate(EntityManager.Instance.GetFromRoster(entityID),
                position, rotation);
            newEntity.gameObject.SetActive(true);
        }
    }
}
