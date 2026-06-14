using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class se : MonoBehaviour
{
	
	[SerializeField] private TMP_Text sond_effect_value;
	[SerializeField] private TMP_Text music_value;

//BOTÕES	
   [SerializeField] private Button return_button;
   [SerializeField] private Button subtract_sond_effect_button;
   [SerializeField] private Button add_sond_effect_button;
   [SerializeField] private Button subtract_music_button;
   [SerializeField] private Button add_music_button;

   void Update()
   {
	   //VALOR DO VOLUME NO PAINEL
	   sond_effect_value.text = master_setting.Instance.sond_effect.ToString();
	   music_value.text = master_setting.Instance.music.ToString();
   }

	void Start()
	{
		

		//BOTÕES DO SISTEMA DE VOLUME
		subtract_sond_effect_button.onClick.AddListener(subtract_sond_effect_game);
		add_sond_effect_button.onClick.AddListener(add_sond_effect_game);
		subtract_music_button.onClick.AddListener(subtract_music_game);
		add_music_button.onClick.AddListener(add_music_game);


		//BOTÃO VOLTAR
		return_button.onClick.AddListener(return_game);
	}

	//EXECUÇÃO DOS BOTÕES DO SISTEMA DE VOLUME
	public void subtract_sond_effect_game()
	{
		if (master_setting.Instance.sond_effect>0)
		{
			master_setting.Instance.sond_effect -=1;
		}
	}

	public void add_sond_effect_game()
	{
		if (master_setting.Instance.sond_effect<10)
		{
			master_setting.Instance.sond_effect +=1;
		}
	}

	public void subtract_music_game()
	{
		if (master_setting.Instance.music>0)
		{
			master_setting.Instance.music -=1;
		}
	}

	public void add_music_game()
	{
		if (master_setting.Instance.music<10)
		{
			master_setting.Instance.music +=1;
		}
	}

	//EXECUÇÃO DO BOTÃO VOLTAR
	public void return_game()
    {
        SceneManager.LoadScene("menu_principal");
    }
}
