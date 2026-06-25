using UnityEngine;

public class master_setting : MonoBehaviour
{
    //VARIAVEIS DE CONFIGURAÇÕES
    public int sond_effect = 10;
	public int music = 10;

    [SerializeField] private int save_setting = 0;
    public int save_communicate
    {
        get
        {
            return save_setting;
        }
        set
        {
            save_setting = value;
        }
    }




    

    //SISTEMA HIGHLANDER
    public static master_setting Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

   


    public void SetEffectVolume(int value)
    {
        sond_effect = Mathf.Clamp(value, 0, 10);

        if (sond_manager.instance != null)
            sond_manager.instance.UpdateVolume();
    }

    public void SetMusicVolume(int value)
    {
        music = Mathf.Clamp(value, 0, 10);

        if (sond_manager.instance != null)
            sond_manager.instance.UpdateVolume();
    }
}