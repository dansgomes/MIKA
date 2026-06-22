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
   [SerializeField] private Button help_keyboard_button;
   [SerializeField] private Button help_gamepad_button;
   [SerializeField] private Button no_help_keyboard_button;
   [SerializeField] private Button no_help_gamepad_button;

   [SerializeField] private GameObject keyboard_view;
   [SerializeField] private GameObject gamepad_view;


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


		//BOTÕES DE AJUDA
		help_keyboard_button.onClick.AddListener(help_keyboard_game);
		help_gamepad_button.onClick.AddListener(help_gamepad_game);
		no_help_keyboard_button.onClick.AddListener(no_help_keyboard_game);
		no_help_gamepad_button.onClick.AddListener(no_help_gamepad_game);
	}

	//EXECUÇÃO DOS BOTÕES DO SISTEMA DE VOLUME
	public void subtract_sond_effect_game()
	{
		if (master_setting.Instance.sond_effect>0)
		{
			master_setting.Instance.SetEffectVolume(
            master_setting.Instance.sond_effect - 1);

			sond_manager.instance.play_sond_effects("pac");
		}
	}

	public void add_sond_effect_game()
	{
		if (master_setting.Instance.sond_effect<10)
		{
			master_setting.Instance.SetEffectVolume(
            master_setting.Instance.sond_effect + 1);

			sond_manager.instance.play_sond_effects("pac");
		}
	}

	public void subtract_music_game()
	{
		if (master_setting.Instance.music>0)
		{
			master_setting.Instance.SetMusicVolume(
            master_setting.Instance.music - 1);

			sond_manager.instance.play_sond_effects("pac");
		}
	}

	public void add_music_game()
	{
		if (master_setting.Instance.music<10)
		{
			master_setting.Instance.SetMusicVolume(
            master_setting.Instance.music + 1);

			sond_manager.instance.play_sond_effects("pac");
		}
	}

	//EXECUÇÃO DO BOTÃO VOLTAR
	public void return_game()
    {
        SceneManager.LoadScene("menu_principal");
		sond_manager.instance.play_sond_effects("pac");
    }

	//EXECUÇÃO DOS BOTÕES DE AJUDA
	public void help_keyboard_game()
	{
		keyboard_view.SetActive(true);
		sond_manager.instance.play_sond_effects("pac");
	}

	public void help_gamepad_game()
	{
		gamepad_view.SetActive(true);
		sond_manager.instance.play_sond_effects("pac");
	}
	public void no_help_keyboard_game()
	{
		keyboard_view.SetActive(false);
		sond_manager.instance.play_sond_effects("pac");
	}

	public void no_help_gamepad_game()
	{
		gamepad_view.SetActive(false);
		sond_manager.instance.play_sond_effects("pac");
	}
}
