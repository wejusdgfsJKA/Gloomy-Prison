using UnityEngine;

public class AnimHandler : MonoBehaviour
{
    public Animator animator { get; protected set; }
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }
    protected void OnEnable()
    {
        animator.SetBool("Winding", false);
        animator.SetBool("Holding", false);
        ChangeSwingAngle(90);
    }
    public void ChangeSwingAngle(float angle)
    {
        angle *= Mathf.Deg2Rad;
        animator.SetFloat("X", Mathf.Cos(angle));
        animator.SetFloat("Y", Mathf.Sin(angle));
    }
    public void Swing()
    {
        animator.SetTrigger("Attack");
        animator.SetBool("Attacking", true);
    }
    public void Feint()
    {
        animator.SetBool("Attacking", false);
    }
    public void StartBlocking()
    {
        animator.SetBool("Blocking", true);
    }
    public void StopBlocking()
    {
        animator.SetBool("Blocking", false);
    }
    public void StopSwing()
    {
        animator.CrossFadeInFixedTime("Base Layer.Idle", 0);
    }
    public void Stagger()
    {
        animator.CrossFadeInFixedTime("Base Layer.Stagger", 0);
    }
}
