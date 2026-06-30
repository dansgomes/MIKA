using UnityEngine;
using UnityEngine.SceneManagement;

public class fase_3 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            master_setting.Instance.save_communicate=2;
            master_setting.Instance.SaveGame();
            SceneManager.LoadScene("Fase 3");
        }
    }
}
