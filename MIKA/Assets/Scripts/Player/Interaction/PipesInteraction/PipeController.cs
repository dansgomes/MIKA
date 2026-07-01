using System.Collections;
using UnityEngine;

public class SequentialActivator : MonoBehaviour
{
    [Header("Objects")]
    public GameObject[] objects;

    [Header("Timing")]
    public float activeDuration = 2f;

    [Header("Settings")]
    public bool playOnStart = true;

    private int currentIndex = 0;
    private Coroutine sequenceCoroutine;

    void Start()
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (playOnStart)
            StartSequence();
    }

    public void StartSequence()
    {
        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        currentIndex = 0;
        sequenceCoroutine = StartCoroutine(SequenceRoutine());
    }

    public void StopSequence()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    private IEnumerator SequenceRoutine()
    {
        while (true)
        {
            if (objects == null || objects.Length == 0) yield break;

            GameObject current = objects[currentIndex];

            if (current != null)
                current.SetActive(false);

            yield return new WaitForSeconds(activeDuration);

            if (current != null)
                current.SetActive(true);

            currentIndex = (currentIndex + 1) % objects.Length;
        }
    }
}