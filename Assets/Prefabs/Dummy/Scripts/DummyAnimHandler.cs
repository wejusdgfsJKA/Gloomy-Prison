using UnityEngine;

public class DummyAnimHandler : MonoBehaviour
{
    protected bool uninteruptible;
    protected Animator animator;
    [SerializeField]
    protected Weapon weapon;
    protected void Awake()
    {
        animator = GetComponent<Animator>();
    }
    protected void OnEnable()
    {
        DisableWeapon();
        uninteruptible = false;
    }
    public void Swing(float angle)
    {
        if (!uninteruptible)
        {
            animator.SetFloat("X", Mathf.Cos(angle * Mathf.Deg2Rad));
            animator.SetFloat("Y", Mathf.Sin(angle * Mathf.Deg2Rad));
            animator.CrossFadeInFixedTime("Swings", 0);
        }
    }
    public void ChangeSwingAngle(float NewAngle)
    {
        animator.SetFloat("X", Mathf.Cos(NewAngle * Mathf.Deg2Rad));
        animator.SetFloat("Y", Mathf.Sin(NewAngle * Mathf.Deg2Rad));
    }
    protected void EnableWeapon()
    {
        weapon.EnableWeapon1();
    }
    protected void AnimStarted()
    {
        uninteruptible = true;
    }
    protected void AnimEnded()
    {
        uninteruptible = false;
    }
    protected void DisableWeapon()
    {
        weapon.DisableWeapon1();
    }
}
