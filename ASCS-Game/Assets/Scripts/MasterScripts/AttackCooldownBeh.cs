using UnityEngine;

public class AttackCooldownBeh : StateMachineBehaviour
{
    public float cooldownTime = 4f;
    public string boolName = "attackCooldownDone";

    private float timer;
    private bool fired;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = cooldownTime;
        fired = false;
        animator.SetBool(boolName, false);
        Debug.Log("[COOLDOWN] State entered. Bool set to FALSE. Timer = " + timer);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (fired) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            fired = true;
            animator.SetBool(boolName, true);
            Debug.Log("[COOLDOWN] Timer done. Bool set to TRUE.");
        }
    }
}