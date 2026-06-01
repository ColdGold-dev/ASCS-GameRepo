using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sword/attack hitbox. Lives on a child of player or enemy.
//
// Damage rules:
//   - Hit stunned enemy -> big damage + big knockback (LAUNCH)
//   - Hit normal enemy  -> attackDamage + normalKnockback
//
// Parry rule (player attacks only):
//   - If the enemy has an active ParryableWindow when we hit, ALSO stun them
public class Attack : MonoBehaviour
{
    [Header("Normal Hit")]
    public int attackDamage = 4;

    [Tooltip("Knockback delivered on normal (non-stunned) hits. " +
             "Set this to zero on the PLAYER's sword (so player doesn't shove enemies on every swing) " +
             "and set it to a real value (e.g. 6,2) on ENEMY attack hitboxes (so they push the player back).")]
    public Vector2 normalKnockback = Vector2.zero;

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
        Vector2 kb;

        if (damageable.IsStunned)
        {
            dmg = stunnedAttackDamage;
            kb = stunnedKnockback;
        }
        else
        {
            dmg = attackDamage;
            kb = normalKnockback;
        }

        // Flip knockback X based on attacker's facing direction
        Vector2 deliveredKnockback = transform.parent.localScale.x > 0
            ? kb
            : new Vector2(-kb.x, kb.y);

        damageable.Hit(dmg, deliveredKnockback);

        // Parry check - only relevant if we ARE the player and target isn't already stunned
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