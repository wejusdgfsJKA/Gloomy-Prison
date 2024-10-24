using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public void Spawn(string _entityName, Vector3 _position, Quaternion _rotation)
    {
        try
        {
            EntityBase _entity = EntityManager.Instance.GetFromPool(_entityName);
            if (_entity != null)
            {
                //we have inactive entities we can reactivate
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
            EntityManager.Instance.AddToPool(_entityName);
            CreateNew(_entityName, _position, _rotation);
        }
    }
    protected void CreateNew(string _entityname, Vector3 _position, Quaternion _rotation)
    {
        try
        {
            //we instantiate a new prefab
            EntityBase _entity = Instantiate(EntityManager.Instance.
                Roster[_entityname], _position, _rotation);
            //register the script in the transform dictionary, to allow damage to
            //pass to it via its transform
            EntityManager.Instance.RegisterEntity(_entity);
            _entity.gameObject.SetActive(true);
        }
        catch (System.Exception _e)
        {
            Debug.Log(_e.Message);
        }
    }
}
