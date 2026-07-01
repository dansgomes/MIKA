using System.Collections;
using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Spawn Settings")]
    public GameObject fallingObjectPrefab;
    public float mapStartX = 0f;
    public float spawnY = 10f;
    public int spawnCountPerInterval = 1;
    public float forwardOffset = 2f;

    [Header("Interval Scaling")]
    public float maxInterval = 3f;
    public float minInterval = 0.3f;
    public float maxDistance = 50f;

    [Header("Settings")]
    public bool spawnOnStart = true;

    private Coroutine spawnCoroutine;

    void Start()
    {
        if (spawnOnStart)
            StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float interval = GetCurrentInterval();
            yield return new WaitForSeconds(interval);

            for (int i = 0; i < spawnCountPerInterval; i++)
            {
                SpawnObject();
            }
        }
    }

    private float GetCurrentInterval()
    {
        if (player == null) return maxInterval;

        float distanceTraveled = Mathf.Max(0f, player.position.x - mapStartX);
        float t = Mathf.Clamp01(distanceTraveled / maxDistance);

        return Mathf.Lerp(maxInterval, minInterval, t);
    }

    private void SpawnObject()
    {
        if (player == null || fallingObjectPrefab == null) return;

        float playerX = player.position.x;

        if (playerX <= mapStartX) return;

        float spawnX = Random.Range(mapStartX, playerX + forwardOffset);

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        Instantiate(fallingObjectPrefab, spawnPosition, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        float playerX = player != null ? player.position.x : mapStartX + 5f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(mapStartX, spawnY - 1f, 0f), new Vector3(mapStartX, spawnY + 1f, 0f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(playerX, spawnY - 1f, 0f), new Vector3(playerX, spawnY + 1f, 0f));

        Gizmos.color = Color.red;
        float maxDistX = mapStartX + maxDistance;
        Gizmos.DrawLine(new Vector3(maxDistX, spawnY - 1f, 0f), new Vector3(maxDistX, spawnY + 1f, 0f));

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        float width = playerX - mapStartX;
        Vector3 center = new Vector3(mapStartX + width / 2f, spawnY, 0f);
        Gizmos.DrawCube(center, new Vector3(width, 0.1f, 0f));
    }
}