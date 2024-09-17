using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager Instance { get; private set; }
    public Dictionary<string, HashSet<Transform>> Targets { get; private set; } = new();
    public Dictionary<string, HashSet<AwarenessSystem>> Listeners { get; private set; } = new();
    public string[] Teams { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void SetTeams(string[] _teams)
    {
        Teams = _teams;
        for (int i = 0; i < _teams.Length; i++)
        {
            if (!Targets.ContainsKey(_teams[i]))
            {
                Targets.Add(_teams[i], new HashSet<Transform>());
            }
            if (!Listeners.ContainsKey(_teams[i]))
            {
                Listeners.Add(_teams[i], new HashSet<AwarenessSystem>());
            }
        }
    }
    public void RegisterTarget(Transform _entity)
    {
        //each team will be identified by a layer
        try
        {
            Targets[_entity.gameObject.layer.ToString()].Add(_entity);
        }
        catch (KeyNotFoundException)
        {
            Targets.Add(_entity.gameObject.layer.ToString(), new HashSet<Transform>());
            Targets[_entity.gameObject.layer.ToString()].Add(_entity);
        }
        //Debug.Log("Registered " + _entity.name + " in team " + _entity.gameObject.layer.ToString());
    }
    public void DeRegisterTarget(Transform _entity)
    {
        try
        {
            Targets[_entity.gameObject.layer.ToString()].Remove(_entity);
        }
        catch (KeyNotFoundException)
        {
            //we don't need to do anything
        }
    }
    public void MakeNoise(Sound _sound)
    {
        //we must notify all relevant listeners
        foreach (var _team in Teams)
        {
            if (_team != _sound.Data.Source.gameObject.layer.ToString())
            {
                //this is an enemy team of the source
                foreach (var _sensor in Listeners[_team])
                {
                    _sensor.Hear(_sound);
                }
            }
        }
    }
    public void RegisterListener(AwarenessSystem sensor)
    {
        try
        {
            Listeners[sensor.transform.root.gameObject.layer.ToString()].Add(sensor);
        }
        catch (KeyNotFoundException)
        {
            Listeners.Add(sensor.transform.root.gameObject.layer.ToString(), new HashSet<AwarenessSystem>());
            Listeners[sensor.transform.root.gameObject.layer.ToString()].Add(sensor);
        }
    }
    public void DeRegisterListener(AwarenessSystem sensor)
    {
        try
        {
            Listeners[sensor.transform.root.gameObject.layer.ToString()].Remove(sensor);
        }
        catch (KeyNotFoundException)
        {
            //we don't need to do anything
        }
    }
}
