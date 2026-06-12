using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class cutscene00 : MonoBehaviour
{
    [SerializeField] private Button nest_button_01;

	void Start()
	{
		nest_button_01.onClick.AddListener(nest_game_01);
	}

	public void nest_game_01()
    {
        SceneManager.LoadScene("cena principal");
    }
}
