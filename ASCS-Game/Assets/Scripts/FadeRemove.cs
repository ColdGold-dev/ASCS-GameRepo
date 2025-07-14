using UnityEngine;

public class FadeRemove : StateMachineBehaviour
{
    public float fadeTime = 0.5f;

    private float timeElapsed = 0f;
    private SpriteRenderer spriteRenderer;
    private GameObject objRemove;
    private Color startColor;

    // Called when the animator first enters the state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        objRemove = animator.gameObject;
        startColor = spriteRenderer.color;
    }

    // Called each frame while in the current animator state
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed += Time.deltaTime;

        float newAlpha = 1 - (timeElapsed / fadeTime);
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

        if (timeElapsed > fadeTime)
        {
            Destroy(objRemove);
        }
    }
}
