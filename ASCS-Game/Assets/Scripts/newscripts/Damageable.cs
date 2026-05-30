using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;
    public UnityEvent<int, int> healthChanged;
    Animator animator;
    [SerializeField] private Animator childAnimator;

    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }

    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);
            if (_health <= 0) IsAlive = false;
        }
    }

    [SerializeField] private bool _isAlive = true;
    [SerializeField] private bool isInvincible = false;
    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    // True while this enemy is frozen by a parry strike.
    // Other scripts (like Attack) read this to decide on bonus damage etc.
    public bool IsStunned { get; set; } = false;

    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);
            if (value == false) damageableDeath.Invoke();
        }
    }

    public bool LockVelocity
    {
        get { return animator.GetBool(AnimationStrings.lockVelocity); }
        set { animator.SetBool(AnimationStrings.lockVelocity, value); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }

        if (LockVelocity && timeSinceHit > invincibilityTime)
        {
            LockVelocity = false;
        }
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            animator.SetTrigger(AnimationStrings.hitTrigger);
            if (childAnimator != null) childAnimator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharecterEvents.charecterDamaged.Invoke(gameObject, damage);
            return true;
        }
        return false;
    }
}