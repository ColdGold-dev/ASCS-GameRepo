using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Normal Hit")]
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    [Header("Hit on Stunned Enemy")]
    [Tooltip("Damage dealt when the target is stunned (parried)")]
    public int stunnedAttackDamage = 25;

    [Tooltip("Knockback applied when the target is stunned")]
    public Vector2 stunnedKnockback = new Vector2(8f, 4f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable == null) return;

        // Decide which damage / knockback set to use based on stun state
        int dmg = damageable.IsStunned ? stunnedAttackDamage : attackDamage;
        Vector2 kb = damageable.IsStunned ? stunnedKnockback : knockback;

        // Flip knockback X if our parent is facing left
        Vector2 deliveredKnockback = transform.parent.localScale.x > 0
            ? kb
            : new Vector2(-kb.x, kb.y);

        bool gotHit = damageable.Hit(dmg, deliveredKnockback, transform.root.gameObject);

        Debug.Log("Calling Hit() on: " + damageable.gameObject.name
                + " | stunned=" + damageable.IsStunned
                + " | dmg=" + dmg);
    }
}