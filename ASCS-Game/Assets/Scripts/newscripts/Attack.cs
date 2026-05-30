using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sword/attack hitbox. Lives on a child object of the player or enemy.
// When it hits a Damageable:
//   - If our parent (the player) is parrying and the target isn't stunned yet -> STUN them
//   - If the target is stunned -> bonus damage
//   - Otherwise -> normal damage
public class Attack : MonoBehaviour
{
    [Header("Normal Hit")]
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    [Header("Hit on Stunned Enemy")]
    public int stunnedAttackDamage = 25;
    public Vector2 stunnedKnockback = new Vector2(8f, 4f);

    // Cache reference to the parry script on the parent (if any). Null for enemy attack hitboxes.
    PlayerParry parry;

    private void Awake()
    {
        // Look up the hierarchy for a PlayerParry. Only the player has one.
        parry = GetComponentInParent<PlayerParry>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable == null) return;

        // CASE 1: Our parent is parrying and this target isn't already stunned -> STUN them
        if (parry != null && parry.IsParrying && !damageable.IsStunned)
        {
            parry.StunEnemy(damageable.gameObject);
            Debug.Log("ATTACK | parry strike on " + damageable.gameObject.name + " - stunning, no damage");
            return; // no damage on the stun hit
        }

        // CASE 2 and 3: Normal hit. Pick damage based on stunned state.
        int dmg = damageable.IsStunned ? stunnedAttackDamage : attackDamage;
        Vector2 kb = damageable.IsStunned ? stunnedKnockback : knockback;

        // Flip knockback X based on facing
        Vector2 deliveredKnockback = transform.parent.localScale.x > 0
            ? kb
            : new Vector2(-kb.x, kb.y);

        damageable.Hit(dmg, deliveredKnockback);

        Debug.Log("ATTACK | hit " + damageable.gameObject.name
                + " | stunned=" + damageable.IsStunned + " | dmg=" + dmg);
    }
}