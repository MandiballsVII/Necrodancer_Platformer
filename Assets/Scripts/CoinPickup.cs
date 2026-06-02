using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        if (startPosition != null)
        {
            transform.position = startPosition +
                Vector3.up * offset;
        }
        else
            return;
    }

    public void Init(Vector2 initialPosition)
    {
        startPosition = initialPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        GameManager.Instance.AddCoin();

        Destroy(gameObject);
    }
}
