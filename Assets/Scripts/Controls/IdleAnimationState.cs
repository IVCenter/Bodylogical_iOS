using UnityEngine;

/// <summary>
/// Loops through random idle animations.
/// </summary>
public class IdleAnimationState : StateMachineBehaviour {
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        animator.SetInteger("IdleRand", Random.Range(0, 4));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.IsName("Default Idle")) {
            animator.SetInteger("IdleRand", Random.Range(0, 4));
        }
    }
}
