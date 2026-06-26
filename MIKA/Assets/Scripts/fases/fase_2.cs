using UnityEngine;
using UnityEngine.SceneManagement;

public class fase_2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            master_setting.Instance.save_communicate=1;
            master_setting.Instance.SaveGame();
        }
    }
}
