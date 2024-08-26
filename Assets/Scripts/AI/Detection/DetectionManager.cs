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
    public void SetTeams(string[] teams)
    {
        Teams = teams;
        for (int i = 0; i < teams.Length; i++)
        {
            if (!Targets.ContainsKey(teams[i]))
            {
                Targets.Add(teams[i], new HashSet<Transform>());
            }
            if (!Listeners.ContainsKey(teams[i]))
            {
                Listeners.Add(teams[i], new HashSet<AwarenessSystem>());
            }
        }
    }
    public void RegisterTarget(Transform entity)
    {
        //each team will be identified by a layer
        try
        {
            Targets[entity.gameObject.layer.ToString()].Add(entity);
        }
        catch (KeyNotFoundException)
        {
            Targets.Add(entity.gameObject.layer.ToString(), new HashSet<Transform>());
            Targets[entity.gameObject.layer.ToString()].Add(entity);
        }
        //Debug.Log("Registered " + entity.name + " in team " + entity.gameObject.layer.ToString());
    }
    public void DeRegisterTarget(Transform entity)
    {
        try
        {
            Targets[entity.gameObject.layer.ToString()].Remove(entity);
        }
        catch (KeyNotFoundException)
        {
            //we don't need to do anything
        }
    }
    public void MakeNoise(Sound sound)
    {
        //we must notify all relevant listeners
        foreach (var team in Teams)
        {
            if (team != sound.Data.Source.gameObject.layer.ToString())
            {
                //this is an enemy team of the source
                foreach (var sensor in Listeners[team])
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
