using UnityEngine;

public class SetFloatOnState : StateMachineBehaviour
{
    [Header("Float parameter name")]
    public string floatName;

    [Header("When to update")]
    public bool updateOnStateEnter = true;
    public bool updateOnStateExit = true;
    public bool updateOnStateMachineEnter = true;
    public bool updateOnStateMachineExit = true;

    [Header("Values to set")]
    public float valueOnEnter = 0f;
    public float valueOnExit = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnStateEnter)
        {
            animator.SetFloat(floatName, valueOnEnter);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnStateExit)
        {
            animator.SetFloat(floatName, valueOnExit);
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachineEnter)
        {
            animator.SetFloat(floatName, valueOnEnter);
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachineExit)
        {
            animator.SetFloat(floatName, valueOnExit);
        }
    }
}
