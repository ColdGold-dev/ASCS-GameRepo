using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Window")]
    public float parryWindow = 2f;

    [Header("Enemy Stun")]
    public float stunDuration = 2f;

    public bool IsParrying { get; private set; } = false;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        StopAllCoroutines();
        StartCoroutine(OpenParryWindow());

        Debug.Log("PARRY-WINDOW OPENED for " + parryWindow + "s");
    }

    IEnumerator OpenParryWindow()
    {
        IsParrying = true;
        yield return new WaitForSeconds(parryWindow);
        IsParrying = false;
        Debug.Log("PARRY-WINDOW CLOSED");
    }

    public void StunAttacker(GameObject attacker)
    {
        if (attacker == null) return;
        Debug.Log("PARRY SUCCESS! Stunning: " + attacker.name);
        StartCoroutine(StunRoutine(attacker));
    }

    IEnumerator StunRoutine(GameObject attacker)
    {
        Rigidbody2D enemyRb = attacker.GetComponent<Rigidbody2D>();
        if (enemyRb == null) yield break;

        Damageable damageable = attacker.GetComponent<Damageable>();

        // Save originals to restore later
        RigidbodyType2D originalType = enemyRb.bodyType;
        RigidbodyConstraints2D originalConstraints = enemyRb.constraints;

        // Disable enemy's behavior scripts
        MonoBehaviour[] behaviors = attacker.GetComponentsInChildren<MonoBehaviour>();
        var disabledScripts = new System.Collections.Generic.List<MonoBehaviour>();
        foreach (MonoBehaviour mb in behaviors)
        {
            if (mb == null) continue;
            if (mb is PlayerParry) continue;
            if (mb is Damageable) continue;
            if (mb.enabled)
            {
                mb.enabled = false;
                disabledScripts.Add(mb);
            }
        }

        // Track whether the stun was broken early by a hit, so we don't double-restore
        bool stunBroken = false;

        // The callback that ends the stun early when the enemy gets hit
        System.Action breakStun = () =>
        {
            if (stunBroken) return; // safety
            stunBroken = true;

            // Restore physics so knockback can land
            if (enemyRb != null)
            {
                enemyRb.constraints = originalConstraints;
                enemyRb.bodyType = originalType;
            }

            // Re-enable scripts
            foreach (MonoBehaviour mb in disabledScripts)
            {
                if (mb != null) mb.enabled = true;
            }

            if (damageable != null)
            {
                damageable.IsStunned = false;
                damageable.OnStunBreak = null;
            }

            Debug.Log("PARRY | stun BROKEN by hit on: " + (attacker != null ? attacker.name : "(destroyed)"));
        };

        // Mark as stunned and register the break callback
        if (damageable != null)
        {
            damageable.IsStunned = true;
            damageable.OnStunBreak = breakStun;
        }

        // Freeze the rigidbody
        enemyRb.linearVelocity = Vector2.zero;
        enemyRb.angularVelocity = 0f;
        enemyRb.bodyType = RigidbodyType2D.Kinematic;
        enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(stunDuration);

        // If the stun wasn't already broken by a hit, restore everything naturally
        if (!stunBroken)
        {
            if (enemyRb != null)
            {
                enemyRb.constraints = originalConstraints;
                enemyRb.bodyType = originalType;
            }

            foreach (MonoBehaviour mb in disabledScripts)
            {
                if (mb != null) mb.enabled = true;
            }

            if (damageable != null)
            {
                damageable.IsStunned = false;
                damageable.OnStunBreak = null;
            }

            Debug.Log("PARRY | stun TIMED OUT on: " + (attacker != null ? attacker.name : "(destroyed)"));
        }
    }
}