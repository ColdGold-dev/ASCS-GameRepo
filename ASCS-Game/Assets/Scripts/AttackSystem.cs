using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackSystem : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float damageAmount = 100f;

    private List<GameObject> targetsInRange = new List<GameObject>();
    private bool isAttacking = false;
    private int targetMaskBits;

    private void Awake()
    {
        targetMaskBits = targetLayerMask.value;
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
        AttackAllTargetsInRange();
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isTargetLayer = ((1 << other.gameObject.layer) & targetMaskBits) != 0;
        if (!isTargetLayer) return;

        if (!targetsInRange.Contains(other.gameObject))
            targetsInRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetMaskBits) != 0)
            targetsInRange.Remove(other.gameObject);
    }

    private void AttackAllTargetsInRange()
    {
        if (targetsInRange.Count == 0)
            return;

        List<GameObject> targetsToRemove = new List<GameObject>();

        foreach (GameObject target in new List<GameObject>(targetsInRange))
        {
            if (target == null)
            {
                targetsToRemove.Add(target);
                continue;
            }
            target.TryGetComponent<Enemy>(out Enemy enemy);
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
            else
            {
                targetsToRemove.Add(target);
            }
        }

        foreach (GameObject target in targetsToRemove)
        {
            targetsInRange.Remove(target);
        }
    }
}