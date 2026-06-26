using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class master_setting : MonoBehaviour
{
    //VARIAVEIS DE CONFIGURAÇÕES
    public int sond_effect = 10;
	public int music = 10;

    private string savePath;

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
        Debug.Log(Application.persistentDataPath);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "/save.dat";

        LoadGame();
    }

    
   //SISTEMA DE SAVE
    public void SaveGame()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(save_setting);
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
            return;

        using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {        
            save_setting = reader.ReadInt32();
        }
    }

    //SISTEMA DE SOM
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