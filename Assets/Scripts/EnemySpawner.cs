using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject skeletonPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float spawnRadius = 12f;
    [SerializeField] private float minPlayerDistance = 4f;

    [Header("Limits")]
    [SerializeField] private int maxEnemiesAlive = 6;

    [Header("Timing")]
    [SerializeField] private float activeMinTime = 5f;
    [SerializeField] private float activeMaxTime = 10f;

    [SerializeField] private float restMinTime = 1f;
    [SerializeField] private float restMaxTime = 5f;

    [SerializeField] private float spawnFrecuency = 2f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask terrainLayer;
    //[SerializeField] private float raycastHeight = 20f;
    [SerializeField] private float raycastDepth = 60f;
    [SerializeField] private bool debugDraw = true;
    [SerializeField] private Vector2 spawnCheckSize = new Vector2(0.5f, 0.5f);

    private readonly List<GameObject> aliveEnemies = new();

    private bool canSpawn = true;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (canSpawn)
        {
            // FASE ACTIVA
            float activeTime = Random.Range(activeMinTime, activeMaxTime);
            float timer = 0f;

            while (timer < activeTime)
            {
                timer += spawnFrecuency;

                TrySpawnOneEnemy();

                yield return new WaitForSeconds(spawnFrecuency);
            }

            // FASE DESCANSO
            float restTime = Random.Range(restMinTime, restMaxTime);
            yield return new WaitForSeconds(restTime);
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
        canSpawn = false;
    }

    private void TrySpawnOneEnemy()
    {
        CleanupDeadEnemies();

        if (aliveEnemies.Count >= maxEnemiesAlive)
            return;

        for (int attempt = 0; attempt < 6; attempt++)
        {
            if (TryFindSpawnPosition(out Vector3 pos))
            {
                GameObject enemy = Instantiate(skeletonPrefab, pos, Quaternion.identity);
                aliveEnemies.Add(enemy);
                return;
            }
        }
    }

    private bool TryFindSpawnPosition(out Vector3 spawnPos)
    {
        Vector2 center = player.position;

        Vector2 randomPoint = center + Random.insideUnitCircle * spawnRadius;

        if (debugDraw)
        {
            Debug.DrawLine(player.position, randomPoint, Color.yellow, 0.1f);
            Debug.DrawRay(randomPoint, Vector2.down * raycastDepth, Color.cyan, 0.5f);
        }

        if (Vector2.Distance(randomPoint, player.position) < minPlayerDistance)
        {
            spawnPos = default;
            return false;
        }

        Vector2 rayOrigin = randomPoint;

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            Vector2.down,
            raycastDepth,
            terrainLayer
        );

        if (debugDraw && hit.collider != null)
        {
            Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.2f);
        }

        if (hit.collider == null)
        {
            spawnPos = default;
            return false;
        }

        Vector3 candidatePos = new Vector3(
            randomPoint.x,
            hit.point.y,
            0f
        );

        bool blocked = Physics2D.OverlapBox(candidatePos + Vector3.up * 1.2f, spawnCheckSize, 0f, terrainLayer);

        Debug.DrawLine(candidatePos, candidatePos + Vector3.up * 2f, Color.magenta, 1f);

        if (blocked)
        {
            spawnPos = default;
            return false;
        }

        if (Vector2.Distance(candidatePos, player.position) < minPlayerDistance)
        {
            spawnPos = default;
            return false;
        }

        // IMPORTANTE: spawn EXACTO sobre el suelo
        spawnPos = candidatePos + Vector3.up * 0.1f;

        if (debugDraw)
        {
            Debug.DrawLine(candidatePos, player.position, Color.red, 0.2f);
        }

        return true;
    }

    private void CleanupDeadEnemies()
    {
        aliveEnemies.RemoveAll(e => e == null);
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, spawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minPlayerDistance);
    }
}