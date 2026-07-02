using System.Collections;
using UnityEngine;
using Cinemachine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [Header("References")]
    public PlayerMovement playerMovement;
    public PlayerDamage playerDamage;
    public Rigidbody2D playerRb;

    [Header("Camera")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera cutsceneCamera;

    public bool IsPlaying { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void BeginCutscene()
    {
        if (IsPlaying) return;
        IsPlaying = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            playerMovement.isKnockedBack = true;
        }

        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        if (cutsceneCamera != null)
            cutsceneCamera.Priority = 20;
        if (playerCamera != null)
            playerCamera.Priority = 10;
    }

    public void EndCutscene()
    {
        if (!IsPlaying) return;
        IsPlaying = false;

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            playerMovement.isKnockedBack = false;
        }

        if (cutsceneCamera != null)
            cutsceneCamera.Priority = 0;
        if (playerCamera != null)
            playerCamera.Priority = 10;
    }

    public IEnumerator MoveCameraTo(Vector3 targetPosition, float duration)
    {
        if (cutsceneCamera == null) yield break;

        Vector3 startPosition = cutsceneCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            cutsceneCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cutsceneCamera.transform.position = targetPosition;
    }
}