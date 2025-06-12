using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attacks : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayerMask;

    private List<GameObject> enemiesInAttackRange = new List<GameObject>();
    private bool isAttacking = false;
    private int enemyMaskBits;

    private void Awake()
    {
        //Problem with layer mask
        //This is the only way me and 03 could get it to work
        if (enemyLayerMask == 0)
            enemyLayerMask = LayerMask.GetMask("Enemy");

        enemyMaskBits = enemyLayerMask;
    }

    public void ActivateAttack(float attackTime)
    {
        if (!isAttacking)
        {
            StartCoroutine(ActivateAttackCoroutine(attackTime));
        }
    }

    private IEnumerator ActivateAttackCoroutine(float attackTime)
    {
        isAttacking = true;
        AttackAllEnemiesInRange();
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isEnemyLayer = ((1 << other.gameObject.layer) & enemyMaskBits) != 0;
        if (!isEnemyLayer) return;

        if (!enemiesInAttackRange.Contains(other.gameObject))
            enemiesInAttackRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyMaskBits) != 0)
            enemiesInAttackRange.Remove(other.gameObject);
    }

    private void AttackAllEnemiesInRange()
    {
        if (enemiesInAttackRange.Count == 0)
            return;

        List<GameObject> enemiesToRemove = new List<GameObject>();

        // Iterate over a copy to avoid modifying the collection during enumeration
        foreach (GameObject enemy in new List<GameObject>(enemiesInAttackRange))
        {
            if (enemy == null)
            {
                enemiesToRemove.Add(enemy);
                continue;
            }

            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                enemyScript.TakeDamage(100f);
            }
            else
            {
                enemiesToRemove.Add(enemy);
            }
        }

        foreach (GameObject enemy in enemiesToRemove)
        {
            enemiesInAttackRange.Remove(enemy);
        }
    }
}

