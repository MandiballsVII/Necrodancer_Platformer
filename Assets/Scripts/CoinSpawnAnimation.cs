using System.Collections;
using UnityEngine;

public class CoinSpawnAnimation : MonoBehaviour
{
    [SerializeField] private float riseHeight = 1f;
    [SerializeField] private float duration = 0.5f;
    CoinPickup coinPickup;

    private void Start()
    {
        StartCoroutine(RiseRoutine());
        coinPickup = GetComponent<CoinPickup>();
    }

    private IEnumerator RiseRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            // Ease Out Cubic
            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.position =
                Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        transform.position = endPos;
        coinPickup.Init(endPos);
    }
}