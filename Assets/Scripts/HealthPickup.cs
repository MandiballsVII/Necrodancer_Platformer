using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Healing")]
    [SerializeField] private int healAmount = 1;

    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.position = startPosition +
            Vector3.up * offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth playerHealth =
            other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        bool healed = playerHealth.Heal(healAmount);

        Destroy(gameObject);
    }
}
