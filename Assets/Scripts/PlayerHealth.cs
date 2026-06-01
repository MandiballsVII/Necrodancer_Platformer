using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 2;
    public int health;
    PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            playerController.Die();
        }
    }
    public bool Heal(int amount)
    {
        if (health >= maxHealth)
            return false;

        health = Mathf.Min(health + amount, maxHealth);

        return true;
    }

    public bool IsFullHealth()
    {
        return health >= maxHealth;
    }
}