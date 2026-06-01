using System.Collections;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [Header("Enemy Stun")]
    public float stunDuration = 2f;
    public string stunnedAnimatorBool = "isStunned";

    [Header("Rewards")]
    [Tooltip("Optional reference to Dash. On successful parry, grants a dash charge.")]
    public Dash dash;

    private void Awake()
    {
        // Try to auto-find Dash on the same GameObject if not assigned
        if (dash == null) dash = GetComponent<Dash>();
    }

    public void StunEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        Damageable d = enemy.GetComponent<Damageable>();
        if (d == null) return;
        if (d.IsStunned) return;

        // Reward: grant a dash charge for this successful parry
        if (dash != null) dash.GrantDashCharge();

        StartCoroutine(StunRoutine(enemy));
    }

    IEnumerator StunRoutine(GameObject enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb == null) yield break;

        Damageable damageable = enemy.GetComponent<Damageable>();
        Animator enemyAnimator = enemy.GetComponent<Animator>();

        RigidbodyType2D originalType = enemyRb.bodyType;
        RigidbodyConstraints2D originalConstraints = enemyRb.constraints;
        Vector3 freezePosition = enemy.transform.position;

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

        if (enemyAnimator != null && !string.IsNullOrEmpty(stunnedAnimatorBool))
        {
            enemyAnimator.SetBool(stunnedAnimatorBool, true);
        }

        bool stunBroken = false;
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

            enemyRb.linearVelocity = Vector2.zero;
            enemyRb.angularVelocity = 0f;
            enemyRb.bodyType = RigidbodyType2D.Kinematic;
            enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;
            enemy.transform.position = freezePosition;

            yield return new WaitForFixedUpdate();
        }

        if (enemyAnimator != null && !string.IsNullOrEmpty(stunnedAnimatorBool))
        {
            enemyAnimator.SetBool(stunnedAnimatorBool, false);
        }

        if (!stunBroken && enemyRb != null)
        {
            enemyRb.constraints = originalConstraints;
            enemyRb.bodyType = originalType;
        }

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