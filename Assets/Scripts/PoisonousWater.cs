using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonousWater : MonoBehaviour
{
    GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        player.GetComponent<PlayerController>().Die();
    }
}
