using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CutsceneTrigger : MonoBehaviour
{
    [Header("Target")]
    public CutsceneBase targetCutscene;

    [Header("Settings")]
    public LayerMask playerLayer;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;
        if (targetCutscene == null) return;

        targetCutscene.Play();
    }
}
