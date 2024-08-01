using UnityEngine;

public class DealerAnimHandler : MonoBehaviour
{
    Animator animator;
    public float x, y;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        animator.SetFloat("X", x);
        animator.SetFloat("Y", y);
    }
    public void Swing()
    {
        animator.SetTrigger("Attack");
    }
}
