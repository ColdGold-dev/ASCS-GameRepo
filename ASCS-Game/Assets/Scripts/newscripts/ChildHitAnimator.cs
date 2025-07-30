using UnityEngine;

public class ChildHitAnimator : MonoBehaviour
{
    [SerializeField] private Damageable parentDamageable;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (parentDamageable == null)
        {
            // Try to auto-find the Damageable in parent
            parentDamageable = GetComponentInParent<Damageable>();
        }

        if (parentDamageable != null)
        {
            parentDamageable.damageableHit.AddListener(OnParentHit);
        }
        else
        {
            Debug.LogWarning("ChildHitAnimator could not find parent Damageable!");
        }
    }

    private void OnDestroy()
    {
        if (parentDamageable != null)
        {
            parentDamageable.damageableHit.RemoveListener(OnParentHit);
        }
    }

    private void OnParentHit(int damage, Vector2 knockback)
    {
        animator.SetTrigger(AnimationStrings.hitTrigger);
    }
}
