using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class game_pause : MonoBehaviour
{
    private InputSystem_Actions playerinput;
    private InputAction pause;

    [SerializeField] private bool pause_game = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject PainelDesiste;

    [SerializeField] private Button continue_button;
    [SerializeField] private Button menu_principal_button;
    [SerializeField] private Button menu_sim_button;
    [SerializeField] private Button menu_nao_button;


    void Start()
    {
        continue_button.onClick.AddListener(continue_game);
        menu_principal_button.onClick.AddListener(menu_principal_game);
        menu_sim_button.onClick.AddListener(menu_sim_game);
        menu_nao_button.onClick.AddListener(menu_nao_game);

        pauseMenu.SetActive(false);
        PainelDesiste.SetActive(false);



        
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
            pause_game = !pause_game;

            Time.timeScale = pause_game ? 0f : 1f;

            pauseMenu.SetActive(pause_game);
        }
    }
}