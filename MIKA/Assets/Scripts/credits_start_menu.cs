using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class credits_start_menu : MonoBehaviour

{
	[SerializeField] private Button return_button;

	void Start()
	{
		return_button.onClick.AddListener(return_game);
	}

	public void return_game()
    {
        SceneManager.LoadScene("menu_principal");
    }
}