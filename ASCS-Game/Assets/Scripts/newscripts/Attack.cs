using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 4;
    public Vector2 normalKnockback = Vector2.zero;

    [Header("Stunned-target bonus")]
    public int stunnedAttackDamage = 25;
    public Vector2 stunnedKnockback = new Vector2(8, 4);

    [Header("Parry direction (set this on player swing hitboxes)")]
    [Tooltip("Set the attack type this swing should match against - only matters for player swings, ignored for enemy hitboxes")]
    public ParryableWindow.AttackType swingDirection = ParryableWindow.AttackType.Mid;

    private PlayerParry parry;

    private void Awake()
    {
        parry = GetComponentInParent<PlayerParry>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable == null) return;

        Vector2 knockback = normalKnockback;
        Vector2 stunnedKb = stunnedKnockback;
        if (transform.parent != null)
        {
            knockback.x *= Mathf.Sign(transform.parent.localScale.x);
            stunnedKb.x *= Mathf.Sign(transform.parent.localScale.x);
        }

        if (damageable.IsStunned)
        {
            damageable.Hit(stunnedAttackDamage, stunnedKb);
        }
        else
        {
            damageable.Hit(attackDamage, knockback);

            // Parry check - only if we're a player swing (have PlayerParry above us)
            if (parry != null)
            {
                ParryableWindow matched = FindMatchingParryableWindow(collision.gameObject);
                if (matched != null)
                {
                    parry.StunEnemy(collision.gameObject);
                }
            }
        }
    }

    private ParryableWindow FindMatchingParryableWindow(GameObject enemy)
    {
        ParryableWindow[] windows = enemy.GetComponentsInChildren<ParryableWindow>(includeInactive: true);
        foreach (ParryableWindow w in windows)
        {
            if (!w.gameObject.activeInHierarchy) continue;
            if (w.attackType == swingDirection)
            {
                return w;
            }
        }
        return null;
    }
}