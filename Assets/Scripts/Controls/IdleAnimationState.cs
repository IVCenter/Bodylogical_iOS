using UnityEngine;

/// <summary>
/// Loops through random idle animations.
/// </summary>
public class IdleAnimationState : StateMachineBehaviour {
    private static readonly int idleRand = Animator.StringToHash("IdleRand");

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        animator.SetInteger(idleRand, Random.Range(0, 4));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.IsName("Default Idle")) {
            animator.SetInteger(idleRand, Random.Range(0, 4));
        }
    }
}
