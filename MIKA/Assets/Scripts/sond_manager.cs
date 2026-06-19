using UnityEngine;
using System.Collections.Generic;

public class sond_manager : MonoBehaviour
{
    public static sond_manager instance;
    private AudioSource sond_effects;
    private AudioSource sond_background;

    public void UpdateVolume()
    {
        sond_effects.volume = master_setting.Instance.sond_effect / 10f;
        sond_background.volume = master_setting.Instance.music / 10f;
    }

    [System.Serializable] public struct SoundClip
    {
        public string soundName;
        public AudioClip soundClip;
    }
    public List <SoundClip> SoundTrack = new List <SoundClip>();
    public List <SoundClip> SoundEffects = new List <SoundClip>();

private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }


public void StopMusic()
{
    sond_background.Stop();
}



void Start()
{
    sond_effects = transform.GetChild(0).GetComponent<AudioSource>();
    sond_background = transform.GetChild(1).GetComponent<AudioSource>();
}
public void play_sond_background(string musicName)
    {
        bool HasFoundMusic = false;
        int musicIndex = -1;
        for (int i=0; i< SoundTrack.Count; i++)
        {
            if (SoundTrack[i].soundName == musicName)
            {
                HasFoundMusic = true;
                musicIndex = i;
            }
        }
        if (HasFoundMusic == true)
        {
            sond_background.clip = SoundTrack[musicIndex].soundClip;
            sond_background.Play();

            sond_background.loop = true;
        }
        else
        {
            Debug.Log("Audio not found");
        }
    }
    public void play_sond_effects(string musicName)
    {
        
        sond_effects = transform.GetChild(0).GetComponent<AudioSource>();
        sond_background = transform.GetChild(1).GetComponent<AudioSource>();
        
        bool HasFoundMusic = false;
        int musicIndex = -1;
        for (int i=0; i< SoundEffects.Count; i++)
        {
            if (SoundEffects[i].soundName == musicName)
            {
                HasFoundMusic = true;
                musicIndex = i;
            }
        }
        if (HasFoundMusic == true)
        {
            sond_effects.PlayOneShot(SoundEffects[musicIndex].soundClip);
        }
        else
        {
            Debug.Log("Audio not found");
        }
    }
}