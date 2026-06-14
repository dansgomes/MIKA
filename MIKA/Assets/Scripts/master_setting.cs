using UnityEngine;

public class master_setting : MonoBehaviour
{
    //VARIAVEIS DE CONFIGURAÇÕES
    public int sond_effect = 10;
	public int music = 10;



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
}