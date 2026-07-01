using System.Collections;
using UnityEngine;

public abstract class CutsceneBase : MonoBehaviour
{
    public bool playOnce = true;
    private bool hasPlayed = false;
    private bool isPlaying = false;

    public void Play()
    {
        if (isPlaying) return;
        if (playOnce && hasPlayed) return;
        if (CutsceneManager.Instance == null) return;
        if (CutsceneManager.Instance.IsPlaying) return;

        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        isPlaying = true;
        hasPlayed = true;

        CutsceneManager.Instance.BeginCutscene();

        yield return StartCoroutine(Execute());

        CutsceneManager.Instance.EndCutscene();

        isPlaying = false;

        OnCutsceneFinished();
    }

    protected abstract IEnumerator Execute();

    protected virtual void OnCutsceneFinished() { }
}
