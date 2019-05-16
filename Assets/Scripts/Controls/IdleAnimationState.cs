using UnityEngine;

/// <summary>
/// Loops through random idle animations.
/// </summary>
public class IdleAnimationState : StateMachineBehaviour {
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        animator.SetInteger("IdleRand", Random.Range(0, 5));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetInteger("IdleRand", Random.Range(0, 5));
    }
}
