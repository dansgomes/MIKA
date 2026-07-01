using System.Collections;
using System.Collections.Generic;
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

    [Header("Spawn Range")]
    public float behindPlayerOffset = 2f;
    public float levelEndX = 100f;

    [Header("Spacing")]
    public float minDistanceBetweenObjects = 3f;

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

            SpawnBatch();
        }
    }

    private float GetCurrentInterval()
    {
        if (player == null) return maxInterval;

        float distanceTraveled = Mathf.Max(0f, player.position.x - mapStartX);
        float t = Mathf.Clamp01(distanceTraveled / maxDistance);

        return Mathf.Lerp(maxInterval, minInterval, t);
    }

    private void SpawnBatch()
    {
        if (player == null || fallingObjectPrefab == null) return;

        float rangeMin = player.position.x - behindPlayerOffset;
        float rangeMax = levelEndX;

        if (rangeMin >= rangeMax) return;

        List<float> placedX = new List<float>();

        for (int i = 0; i < spawnCountPerInterval; i++)
        {
            float spawnX = Random.Range(rangeMin, rangeMax);

            if (IsTooClose(spawnX, placedX))
                continue;

            Vector2 spawnPosition = new Vector2(spawnX, spawnY);
            Instantiate(fallingObjectPrefab, spawnPosition, Quaternion.identity);

            placedX.Add(spawnX);
        }
    }

    private bool IsTooClose(float x, List<float> placedX)
    {
        foreach (float other in placedX)
        {
            if (Mathf.Abs(x - other) < minDistanceBetweenObjects)
                return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        float playerX = player != null ? player.position.x : mapStartX + 5f;
        float rangeMin = playerX - behindPlayerOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(mapStartX, spawnY - 1f, 0f), new Vector3(mapStartX, spawnY + 1f, 0f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(playerX, spawnY - 1f, 0f), new Vector3(playerX, spawnY + 1f, 0f));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(levelEndX, spawnY - 1f, 0f), new Vector3(levelEndX, spawnY + 1f, 0f));

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        float width = levelEndX - rangeMin;
        if (width > 0f)
        {
            Vector3 center = new Vector3(rangeMin + width / 2f, spawnY, 0f);
            Gizmos.DrawCube(center, new Vector3(width, 0.1f, 0f));
        }
    }
}