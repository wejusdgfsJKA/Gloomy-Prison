using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager Instance { get; protected set; }
    /// <summary>
    /// All targets which can be heard.
    /// </summary>
    public Dictionary<int, HashSet<Transform>> Targets { get; protected set; } = new();
    /// <summary>
    /// All sensors which can hear noises.
    /// </summary>
    public Dictionary<int, HashSet<AwarenessSystem>> Listeners { get; protected set; } = new();
    public List<int> Teams { get; protected set; } = new();
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    /// <summary>
    /// This entity can now be detected.
    /// </summary>
    /// <param name="entity">The entity in question.</param>
    public void RegisterTarget(Transform entity)
    {
        //each team will be identified by a layer
        HashSet<Transform> targets;
        if (Targets.TryGetValue(entity.gameObject.layer, out targets))
        {
            targets.Add(entity);
        }
        else
        {
            Teams.Add(entity.gameObject.layer);
            targets = new HashSet<Transform>() { entity };
            Targets.Add(entity.gameObject.layer, targets);
        }
    }
    /// <summary>
    /// This target can no longer be detected, likely because it has been destroyed.
    /// </summary>
    /// <param name="entity">The entity that can no longer be detected.</param>
    public void DeRegisterTarget(Transform entity)
    {
        HashSet<Transform> targets;
        if (Targets.TryGetValue(entity.gameObject.layer, out targets))
        {
            targets.Remove(entity);
        }
    }
    /// <summary>
    /// Make a noise which will be picked up by any sensors which are not on the 
    /// same team as the source of the sound.
    /// </summary>
    /// <param name="sound">The sound being emitted.</param>
    public void MakeNoise(Sound sound)
    {
        //we must notify all relevant listeners
        HashSet<AwarenessSystem> listeners;
        for (int i = 0; i < Teams.Count; i++)
        {
            if (Teams[i] != sound.Data.Source.gameObject.layer)
            {
                if (Listeners.TryGetValue(Teams[i], out listeners))
                {
                    foreach (var listener in listeners)
                    {
                        listener.Hear(sound);
                    }
                }
            }
        }
    }
    /// <summary>
    /// This sensor is now active.
    /// </summary>
    /// <param name="sensor">The sensor in question.</param>
    public void RegisterListener(AwarenessSystem sensor)
    {
        //each team will be identified by a layer
        HashSet<AwarenessSystem> listeners;
        if (Listeners.TryGetValue(sensor.gameObject.layer, out listeners))
        {
            listeners.Add(sensor);
        }
        else
        {
            Teams.Add(sensor.gameObject.layer);
            listeners = new HashSet<AwarenessSystem>() { sensor };
            Listeners.Add(sensor.gameObject.layer, listeners);
        }
    }
    /// <summary>
    /// This sensor is no longer active.
    /// </summary>
    /// <param name="sensor">The sensor in question.</param>
    public void DeRegisterListener(AwarenessSystem sensor)
    {
        HashSet<AwarenessSystem> listeners;
        if (Listeners.TryGetValue(sensor.gameObject.layer, out listeners))
        {
            listeners.Remove(sensor);
        }
    }
}
