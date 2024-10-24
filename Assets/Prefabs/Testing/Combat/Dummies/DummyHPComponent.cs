using UnityEngine;

public class DummyHPComponent : HPComponent
{
    protected void Awake()
    {
        TakeDamage = (int _damage) =>
        {
            Debug.Log("Damage taken: " + _damage + ".");
        };
    }
}
