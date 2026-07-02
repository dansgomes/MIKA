using System.Collections;
using UnityEngine;

public class CutsceneOpening : CutsceneBase
{
    [Header("Timing")]
    public float skyHoldDuration = 1.5f;

    public float cameraBlendDuration = 1.5f;

    public float postBlendHold = 2f;

    [Header("Auto Start")]
    public bool playOnStart = true;

    void Start()
    {
        if (playOnStart)
            StartCoroutine(AutoPlay());
    }

    private IEnumerator AutoPlay()
    {
        yield return null;
        Play();
    }

    protected override IEnumerator Execute()
    {
        yield return new WaitForSeconds(skyHoldDuration);

        CutsceneManager.Instance.playerCamera.Priority = 20;
        CutsceneManager.Instance.cutsceneCamera.Priority = 10;

        yield return new WaitForSeconds(cameraBlendDuration);

        yield return new WaitForSeconds(postBlendHold);
    }
}