using UnityEngine;
public class WalkManager : MonoBehaviour
{
    protected Animator animator;
    protected void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Walk(Vector2 direction)
    {
        animator.SetFloat("Y", direction.y);
        animator.SetFloat("X", direction.x);
    }
}
