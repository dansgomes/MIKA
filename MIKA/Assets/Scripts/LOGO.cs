using UnityEngine;
using UnityEngine.SceneManagement;

public class LOGO : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(MeuEvento), 3f);
    }

    void MeuEvento()
    {
        SceneManager.LoadScene("menu_principal");
    }
}