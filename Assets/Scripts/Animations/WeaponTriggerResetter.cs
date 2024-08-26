using UnityEngine;
//this script resets the triggers for weapon animations in case
//they remain active somehow
public class WeaponTriggerResetter : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Feint");
        animator.ResetTrigger("Windup");
    }
}
