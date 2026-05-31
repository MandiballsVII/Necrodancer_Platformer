using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 2;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Debug.Log("Player dead");
        }
    }
}