using UnityEngine;

public class HideAndSeekStart : MonoBehaviour
{
    [SerializeField] protected Transform hunterSpawn, targetSpawn;
    private void Start()
    {
        EntityManager.Instance.Spawn("Hunter", hunterSpawn.position, hunterSpawn.rotation);
        //EntityManager.Instance.Spawn("Target", hunterSpawn.position, hunterSpawn.rotation);
    }
}
