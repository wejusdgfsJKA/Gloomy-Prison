using UnityEngine;

public class CombatStart : MonoBehaviour
{
    protected EntitySpawner spawner;
    public Transform DaveSpawn, DummySpawn;
    public bool Dave, Dummy;
    private void Awake()
    {
        spawner = GetComponent<EntitySpawner>();
    }
    private void Update()
    {
        if (Dave)
        {
            //spawner.Spawn("Dave", DaveSpawn.position, DaveSpawn.rotation);
            Dave = false;
        }
        if (Dummy)
        {
            //spawner.Spawn("EvilDave", DummySpawn.position, DummySpawn.rotation);
            Dummy = false;
        }
    }
}
