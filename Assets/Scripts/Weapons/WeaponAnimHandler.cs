using UnityEngine;
[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(Animator))]
public class WeaponAnimHandler : MonoBehaviour
{
    protected Animator animator;
    protected float angle;//attack angle
    protected void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void Release()
    {
        animator.SetBool("Release", true);
    }
    public void Feint()
    {
        animator.SetTrigger("Feint");
    }
    public void SetBlock(bool val)
    {
        animator.SetBool("Block", val);
    }
    public void SetSwing(bool val)
    {
        animator.SetBool("Swing", val);
    }
    public void Windup()
    {
        animator.SetTrigger("Windup");
    }
    public void Push()
    {
        animator.SetTrigger("Push");
    }
    public void ChangeAngle(float newangle)
    {
        angle = newangle * Mathf.Deg2Rad;
        animator.SetFloat("X", Mathf.Cos(angle));
        animator.SetFloat("Y", Mathf.Sin(angle));
    }
}
