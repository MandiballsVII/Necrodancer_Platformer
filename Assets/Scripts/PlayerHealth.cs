using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 2;
    public int health;

    public event Action<int> OnHealthChanged;

    PlayerController playerController;

    [SerializeField] private float invulnerabilityTime = 2.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    public bool isInvulnerable;

    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        health = maxHealth;
        OnHealthChanged?.Invoke(health);
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
            return;

        health -= damage;
        OnHealthChanged?.Invoke(health);

        if (health <= 0)
        {
            playerController.Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            true
        );

        float timer = 0f;

        while (timer < invulnerabilityTime)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval * 2f;
        }

        spriteRenderer.enabled = true;

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            false
        );

        isInvulnerable = false;
    }

    public bool Heal(int amount)
    {
        if (health >= maxHealth)
            return false;

        health = Mathf.Min(health + amount, maxHealth);
        OnHealthChanged?.Invoke(health);

        return true;
    }
}