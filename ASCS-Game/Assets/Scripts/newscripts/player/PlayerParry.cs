using System.Collections;
using UnityEngine;

// Owns the stun behavior. Parry timing is controlled by the enemy's
// ParryableWindow child GameObject - their attack animation toggles it.
// Attack.cs checks that and calls StunEnemy on us when appropriate.
public class PlayerParry : MonoBehaviour
{
    [Header("Enemy Stun")]
    public float stunDuration = 2f;

    public void StunEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        Damageable d = enemy.GetComponent<Damageable>();
        if (d == null) return;
        if (d.IsStunned) return;

        StartCoroutine(StunRoutine(enemy));
    }

    IEnumerator StunRoutine(GameObject enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb == null) yield break;

        Damageable damageable = enemy.GetComponent<Damageable>();

        RigidbodyType2D originalType = enemyRb.bodyType;
        RigidbodyConstraints2D originalConstraints = enemyRb.constraints;
        Vector3 freezePosition = enemy.transform.position;

        // Disable behavior scripts (keep Damageable enabled so they can still take hits)
        MonoBehaviour[] behaviors = enemy.GetComponentsInChildren<MonoBehaviour>();
        var disabledScripts = new System.Collections.Generic.List<MonoBehaviour>();
        foreach (MonoBehaviour mb in behaviors)
        {
            if (mb == null) continue;
            if (mb is Damageable) continue;
            if (mb.enabled)
            {
                mb.enabled = false;
                disabledScripts.Add(mb);
            }
        }

        // Disable enemy attack hitboxes so they can't damage the player during stun
        Attack[] attackHitboxes = enemy.GetComponentsInChildren<Attack>();
        var disabledHitboxObjects = new System.Collections.Generic.List<GameObject>();
        foreach (Attack atk in attackHitboxes)
        {
            if (atk == null) continue;
            if (atk.gameObject.activeSelf)
            {
                atk.gameObject.SetActive(false);
                disabledHitboxObjects.Add(atk.gameObject);
            }
        }

        bool stunBroken = false;

        // Callback registered with Damageable - when the stunned enemy gets hit,
        // restore physics so the knockback can actually launch them.
        System.Action breakStun = () =>
        {
            if (stunBroken) return;
            stunBroken = true;

            if (enemyRb != null)
            {
                enemyRb.constraints = originalConstraints;
                enemyRb.bodyType = originalType;
            }
        };

        if (damageable != null)
        {
            damageable.IsStunned = true;
            damageable.OnStunHit = breakStun;
        }

        float startTime = Time.time;
        while (Time.time - startTime < stunDuration && !stunBroken)
        {
            if (enemy == null || enemyRb == null) yield break;

            // Re-apply the freeze every physics step to override any script trying to move
            enemyRb.linearVelocity = Vector2.zero;
            enemyRb.angularVelocity = 0f;
            enemyRb.bodyType = RigidbodyType2D.Kinematic;
            enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;
            enemy.transform.position = freezePosition;

            yield return new WaitForFixedUpdate();
        }

        // Restore rigidbody if not already done by breakStun
        if (!stunBroken && enemyRb != null)
        {
            enemyRb.constraints = originalConstraints;
            enemyRb.bodyType = originalType;
        }

        // Re-enable scripts and hitboxes
        foreach (MonoBehaviour mb in disabledScripts)
        {
            if (mb != null) mb.enabled = true;
        }

        foreach (GameObject hitbox in disabledHitboxObjects)
        {
            if (hitbox != null) hitbox.SetActive(true);
        }

        if (damageable != null)
        {
            damageable.IsStunned = false;
            damageable.OnStunHit = null;
        }
    }
}