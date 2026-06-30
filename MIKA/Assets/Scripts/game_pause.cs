using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class game_pause : MonoBehaviour
{
    [SerializeField] private TMP_Text sond_effect_value;
	[SerializeField] private TMP_Text music_value;

    private InputSystem_Actions playerinput;
    private InputAction pause;

    [SerializeField] private bool pause_game = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject PainelDesiste;

    [SerializeField] private Button continue_button;
    [SerializeField] private Button menu_principal_button;
    [SerializeField] private Button menu_sim_button;
    [SerializeField] private Button menu_nao_button;

    [SerializeField] private Button help_keyboard_button;
    [SerializeField] private Button help_gamepad_button;
    [SerializeField] private Button no_help_keyboard_button;
    [SerializeField] private Button no_help_gamepad_button;

    [SerializeField] private Button subtract_sond_effect_button;
    [SerializeField] private Button add_sond_effect_button;
    [SerializeField] private Button subtract_music_button;
    [SerializeField] private Button add_music_button;

    [SerializeField] private GameObject keyboard_view;
    [SerializeField] private GameObject gamepad_view;


    void Start()
    {
        
		subtract_sond_effect_button.onClick.AddListener(subtract_sond_effect_game);
		add_sond_effect_button.onClick.AddListener(add_sond_effect_game);
		subtract_music_button.onClick.AddListener(subtract_music_game);
		add_music_button.onClick.AddListener(add_music_game);


        continue_button.onClick.AddListener(continue_game);
        menu_principal_button.onClick.AddListener(menu_principal_game);

        menu_sim_button.onClick.AddListener(menu_sim_game);
        menu_nao_button.onClick.AddListener(menu_nao_game);

        help_keyboard_button.onClick.AddListener(help_keyboard_game);
		help_gamepad_button.onClick.AddListener(help_gamepad_game);
		no_help_keyboard_button.onClick.AddListener(no_help_keyboard_game);
		no_help_gamepad_button.onClick.AddListener(no_help_gamepad_game);

        pauseMenu.SetActive(false);
        PainelDesiste.SetActive(false);
        keyboard_view.SetActive(false);
        gamepad_view.SetActive(false);
     
    }
    
    
    

    public void menu_sim_game()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("menu_principal");
    }

    public void menu_nao_game()
    {
        PainelDesiste.SetActive(false);
    }

    public void continue_game()
    {
        pause_game = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void menu_principal_game()
    {
        
        PainelDesiste.SetActive(true);

    }


    void Awake()
    {
        playerinput = new InputSystem_Actions();
        pause = playerinput.player.pause;
    }

    void OnEnable()
    {
        playerinput.player.Enable();
    }

    void OnDisable()
    {
        playerinput.player.Disable();
    }

    void Update()
    {
        if (pause.WasPressedThisFrame())
        {
            Debug.Log("pause");
            pause_game = !pause_game;

            Time.timeScale = pause_game ? 0f : 1f;

            pauseMenu.SetActive(pause_game);
        }

        {
	    //VALOR DO VOLUME NO PAINEL
	    //sond_effect_value.text = master_setting.Instance.sond_effect.ToString();
	    //music_value.text = master_setting.Instance.music.ToString();
        }
    }

    //EXECUÇÃO DOS BOTÕES DO SISTEMA DE VOLUME
	public void subtract_sond_effect_game()
	{
		//if (master_setting.Instance.sond_effect>0)
		//{
		//	master_setting.Instance.SetEffectVolume(
        //    master_setting.Instance.sond_effect - 1);

		//	sond_manager.instance.play_sond_effects("pac");
		//}
	}

	public void add_sond_effect_game()
	{
		//if (master_setting.Instance.sond_effect<10)
		//{
		//	master_setting.Instance.SetEffectVolume(
        //    master_setting.Instance.sond_effect + 1);

		//	sond_manager.instance.play_sond_effects("pac");
		//}
	}

	public void subtract_music_game()
	{
		//if (master_setting.Instance.music>0)
		//{
		//	master_setting.Instance.SetMusicVolume(
        //    master_setting.Instance.music - 1);

		//	sond_manager.instance.play_sond_effects("pac");
		//}
	}

	public void add_music_game()
	{
		//if (master_setting.Instance.music<10)
		//{
		//	  master_setting.Instance.SetMusicVolume(
        //    master_setting.Instance.music + 1);

		//	sond_manager.instance.play_sond_effects("pac");
		//}
	}

    //EXECUÇÃO DOS BOTÕES DE AJUDA
	public void help_keyboard_game()
	{
		keyboard_view.SetActive(true);
		//sond_manager.instance.play_sond_effects("pac");
	}

	public void help_gamepad_game()
	{
		gamepad_view.SetActive(true);
		//sond_manager.instance.play_sond_effects("pac");
	}
	public void no_help_keyboard_game()
	{
		keyboard_view.SetActive(false);
		//sond_manager.instance.play_sond_effects("pac");
	}

	public void no_help_gamepad_game()
	{
		gamepad_view.SetActive(false);
		//sond_manager.instance.play_sond_effects("pac");
	}
}