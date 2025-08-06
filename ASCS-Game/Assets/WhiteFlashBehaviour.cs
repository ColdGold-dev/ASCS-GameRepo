using UnityEngine;

public class WhiteFlashBehaviour : StateMachineBehaviour
{
    [SerializeField] private Color flashColor = Color.white;
    // [SerializeField] private float flashDuration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = animator.GetComponent<SpriteRenderer>() ?? animator.GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}
