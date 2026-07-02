using UnityEngine;

public class LeverFallSequence : MonoBehaviour
{
    [Header("Platforms")]
    public FallingPlatform[] platforms;

    [Header("Player")]
    public PlayerAnimator playerAnimator;

    public void TriggerFall()
    {
        foreach (var platform in platforms)
        {
            if (platform != null)
                platform.Fall();
        }

        playerAnimator.SetOneHanded();
    }
}