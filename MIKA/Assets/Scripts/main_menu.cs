using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{
    [SerializeField] private Button start_button;
    [SerializeField] private Button continue_button;
    [SerializeField] private Button settings_button;
    [SerializeField] private Button credits_button;
    [SerializeField] private Button exit_button;

    void Start()
    {
        start_button.onClick.AddListener(start_game);
        continue_button.onClick.AddListener(continue_game);
        settings_button.onClick.AddListener(settings_game);
        credits_button.onClick.AddListener(credits_game);
        exit_button.onClick.AddListener(exit_game);
    }
    //BOTÃO JOGAR
    public void start_game()
    {
        SceneManager.LoadScene("cutscene_00");
        sond_manager.instance.play_sond_effects("pac");
    }
    
    //BOTÃO CONTINUAR
    public void continue_game()
    {
        sond_manager.instance.play_sond_effects("pac");
    }

    //BOTÃO CONFIGURAÇÕES
    public void settings_game()
    {
        SceneManager.LoadScene("setting_menu_scene");
        sond_manager.instance.play_sond_effects("pac");
    }
    
    //BOTÃO CRÉDITOS
    public void credits_game()
    {
        SceneManager.LoadScene("credits_scene");
        sond_manager.instance.play_sond_effects("pac");
    }
        
    //BOTÃO SAIR
    public void exit_game()
    {
        sond_manager.instance.play_sond_effects("pac");
        Application.Quit();
    }
}
