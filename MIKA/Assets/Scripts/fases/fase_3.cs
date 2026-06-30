using UnityEngine;
using UnityEngine.SceneManagement;

public class fase_3 : MonoBehaviour
{
    public GameObject master_setting_prefab;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(master_setting.Instance == null)
            {
                Instantiate(master_setting_prefab);
            }
            master_setting.Instance.save_communicate=2;
            master_setting.Instance.SaveGame();
            SceneManager.LoadScene("Fase 3");
        }
    }
}
