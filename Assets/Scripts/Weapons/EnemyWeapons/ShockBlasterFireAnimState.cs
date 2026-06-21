using UnityEngine;

public class ShockBlasterFireAnimState : StateMachineBehaviour {
    [SerializeField] protected float normalizedDamageStart = 0f;
    [SerializeField] protected float normalizedDamageEnd = 1f;

    protected ShockBlaster blaster;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        blaster = animator.GetComponentInParent<ShockBlaster>();
        blaster.UpdateHitbox(true);;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        blaster.UpdateHitbox(false);
    }
}
