using UnityEngine;
using UnityEngine.SceneManagement;

public class end_game : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("LOGO");
        }
    }
}