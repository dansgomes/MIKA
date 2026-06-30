using UnityEngine;
using UnityEngine.SceneManagement;

public class fase_6 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            master_setting.Instance.save_communicate=5;
            master_setting.Instance.SaveGame();
            SceneManager.LoadScene("Fase 6");
        }
    }
}
