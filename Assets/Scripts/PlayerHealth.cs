using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 2;
    PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            playerController.Die();
        }
    }
}