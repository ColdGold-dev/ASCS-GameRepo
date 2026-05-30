using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Opens a brief "perfect strike" window when Attack is pressed.
// Attack.cs checks this window when the sword lands on an enemy.
public class PlayerParry : MonoBehaviour
{
    [Header("Parry Window")]
    [Tooltip("How long after pressing Attack the parry window stays active (seconds)")]
    public float parryWindow = 0.3f;

    [Header("Enemy Stun")]
    [Tooltip("How long an enemy stays frozen when parry-hit")]
    public float stunDuration = 2f;

    public bool IsParrying { get; private set; } = false;

    Coroutine parryWindowRoutine;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (parryWindowRoutine != null) StopCoroutine(parryWindowRoutine);
        parryWindowRoutine = StartCoroutine(OpenParryWindow());

        Debug.Log("PARRY-WINDOW OPENED for " + parryWindow + "s");
    }

    IEnumerator OpenParryWindow()
    {
        IsParrying = true;
        yield return new WaitForSeconds(parryWindow);
        IsParrying = false;
    }

    // Called by Attack.cs when our sword hits an enemy during the parry window.
    public void StunEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        Damageable d = enemy.GetComponent<Damageable>();
        if (d == null) return;
        if (d.IsStunned) return; // already stunned, don't restart

        Debug.Log("PARRY STRIKE! Stunning: " + enemy.name);
        StartCoroutine(StunRoutine(enemy));
    }

    IEnumerator StunRoutine(GameObject enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb == null) yield break;

        Damageable damageable = enemy.GetComponent<Damageable>();

        // Save originals for restore
        RigidbodyType2D originalType = enemyRb.bodyType;
        RigidbodyConstraints2D originalConstraints = enemyRb.constraints;
        Vector3 freezePosition = enemy.transform.position;

        // Disable all behavior scripts (AI, movement, etc.) - keep Damageable
        // so the enemy can still take hits during the stun.
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

        if (damageable != null) damageable.IsStunned = true;

        // Force-hold the freeze every physics step.
        // This wins against any other script trying to move the rigidbody.
        float startTime = Time.time;
        while (Time.time - startTime < stunDuration)
        {
            if (enemy == null || enemyRb == null) yield break;

            enemyRb.linearVelocity = Vector2.zero;
            enemyRb.angularVelocity = 0f;
            enemyRb.bodyType = RigidbodyType2D.Kinematic;
            enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;
            enemy.transform.position = freezePosition;

            yield return new WaitForFixedUpdate();
        }

        // Restore everything
        if (enemyRb != null)
        {
            enemyRb.constraints = originalConstraints;
            enemyRb.bodyType = originalType;
        }

        foreach (MonoBehaviour mb in disabledScripts)
        {
            if (mb != null) mb.enabled = true;
        }

        if (damageable != null) damageable.IsStunned = false;

        Debug.Log("PARRY | stun ended");
    }
}