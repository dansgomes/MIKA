using System.Collections;
using UnityEngine;

public class CutsceneTransition : CutsceneBase
{
    [Header("References")]
    public PlayerAnimator playerAnimator;

    [Header("Camera")]
    public Transform focusPoint;
    public float moveInDuration = 1f;
    public float holdDuration = 2f;
    public float moveOutDuration = 1f;

    protected override IEnumerator Execute()
    {
        if (focusPoint != null)
        {
            yield return StartCoroutine(
                CutsceneManager.Instance.MoveCameraTo(focusPoint.position, moveInDuration)
            );
        }

        yield return new WaitForSeconds(holdDuration);

        if (playerAnimator != null)
            playerAnimator.SetOneHanded();

        yield return new WaitForSeconds(moveOutDuration);
    }

    protected override void OnCutsceneFinished()
    {
    }
}
