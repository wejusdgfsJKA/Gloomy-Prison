using UnityEngine;

public class sc : MonoBehaviour
{
    [SerializeField]
    Weapon weapon;
    private void OnEnable()
    {
        weapon.EnableWeapon();
    }
    private void OnDisable()
    {
        weapon.DisableWeapon();
    }
}
