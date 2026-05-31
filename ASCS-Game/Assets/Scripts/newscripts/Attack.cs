using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sword/attack hitbox. Lives on a child of player or enemy.
//
// Damage rules:
//   - Hit stunned enemy -> big damage + big knockback (LAUNCH)
//   - Hit normal enemy  -> small damage + ZERO knockback (no push)
//
// Parry rule (player attacks only):
//   - If the enemy has an active ParryableWindow when we hit, ALSO stun them
public class Attack : MonoBehaviour
{
    [Header("Normal Hit")]
    public int attackDamage = 4;

    // No knockback field for normal hits anymore - we always send zero.
    // Stunned hits still use the bonus knockback below.

    [Header("Hit on Stunned Enemy")]
    public int stunnedAttackDamage = 25;
    public Vector2 stunnedKnockback = new Vector2(8f, 4f);

    PlayerParry parry;

    private void Awake()
    {
        parry = GetComponentInParent<PlayerParry>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable == null) return;

        int dmg;
        Vector2 deliveredKnockback;

        if (damageable.IsStunned)
        {
            // Stunned hit - bonus damage and knockback (flip X based on facing)
            dmg = stunnedAttackDamage;
            deliveredKnockback = transform.parent.localScale.x > 0
                ? stunnedKnockback
                : new Vector2(-stunnedKnockback.x, stunnedKnockback.y);
        }
        else
        {
            // Normal hit - small damage, NO knockback
            dmg = attackDamage;
            deliveredKnockback = Vector2.zero;
        }

        damageable.Hit(dmg, deliveredKnockback);

        // Parry check - only relevant if we're the player and target isn't already stunned
        if (parry != null && !damageable.IsStunned)
        {
            if (IsEnemyParryable(damageable.gameObject))
            {
                parry.StunEnemy(damageable.gameObject);
            }
        }
    }

    bool IsEnemyParryable(GameObject enemy)
    {
        ParryableWindow[] windows = enemy.GetComponentsInChildren<ParryableWindow>(includeInactive: true);
        foreach (ParryableWindow w in windows)
        {
            if (w == null) continue;
            if (w.gameObject.activeInHierarchy) return true;
        }
        return false;
    }
}