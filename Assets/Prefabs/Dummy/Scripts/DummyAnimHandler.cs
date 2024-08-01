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
        weapon.enabled = false;
        uninteruptible = false;
    }
    public void Swing(float angle)
    {
        ChangeSwingAngle(angle);
        animator.SetTrigger("Attack");
        animator.SetBool("Attacking", true);
    }
    public void ChangeSwingAngle(float NewAngle)
    {
        animator.SetFloat("X", Mathf.Cos(NewAngle * Mathf.Deg2Rad));
        animator.SetFloat("Y", Mathf.Sin(NewAngle * Mathf.Deg2Rad));
    }
    protected void AnimStarted()
    {
        uninteruptible = true;
    }
    protected void AnimEnded()
    {
        uninteruptible = false;
    }
}
