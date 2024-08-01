using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager instance { get; private set; }
    public Dictionary<string, HashSet<Transform>> targets { get; private set; } = new();
    public Dictionary<string, HashSet<AwarenessSystem>> listeners { get; private set; } = new();
    public string[] teams { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void SetTeams(string[] teams)
    {
        this.teams = teams;
        for (int i = 0; i < teams.Length; i++)
        {
            if (!targets.ContainsKey(teams[i]))
            {
                targets.Add(teams[i], new HashSet<Transform>());
            }
            if (!listeners.ContainsKey(teams[i]))
            {
                listeners.Add(teams[i], new HashSet<AwarenessSystem>());
            }
        }
    }
    public void RegisterTarget(Transform entity)
    {
        //each team will be identified by a layer
        try
        {
            targets[entity.gameObject.layer.ToString()].Add(entity);
        }
        catch (KeyNotFoundException)
        {
            targets.Add(entity.gameObject.layer.ToString(), new HashSet<Transform>());
            targets[entity.gameObject.layer.ToString()].Add(entity);
        }
        //Debug.Log("Registered " + entity.name + " in team " + entity.gameObject.layer.ToString());
    }
    public void DeRegisterTarget(Transform entity)
    {
        try
        {
            targets[entity.gameObject.layer.ToString()].Remove(entity);
        }
        catch (KeyNotFoundException)
        {
            //we don't need to do anything
        }
    }
    public void MakeNoise(Sound sound)
    {
        //we must notify all relevant listeners
        foreach (var team in teams)
        {
            if (team != sound.data.source.gameObject.layer.ToString())
            {
                //this is an enemy team of the source
                foreach (var sensor in listeners[team])
                {
                    sensor.Hear(sound);
                }
            }
        }
    }
    public void RegisterListener(AwarenessSystem sensor)
    {
        try
        {
            listeners[sensor.transform.root.gameObject.layer.ToString()].Add(sensor);
        }
        catch (KeyNotFoundException)
        {
            listeners.Add(sensor.transform.root.gameObject.layer.ToString(), new HashSet<AwarenessSystem>());
            listeners[sensor.transform.root.gameObject.layer.ToString()].Add(sensor);
        }
    }
    public void DeRegisterListener(AwarenessSystem sensor)
    {
        try
        {
            listeners[sensor.transform.root.gameObject.layer.ToString()].Remove(sensor);
        }
        catch (KeyNotFoundException)
        {
            //we don't need to do anything
        }
    }
}
