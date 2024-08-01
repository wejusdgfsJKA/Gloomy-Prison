using UnityEngine;

public class BlockInputReceiver : MonoBehaviour
{
    [SerializeField]
    protected Weapon weapon;
    private void OnEnable()
    {
        weapon.StartBlocking();
    }
    private void OnDisable()
    {
        weapon.StopBlocking();
    }
}
