using UnityEngine;
using System.Collections.Generic;

public class sond_manager : MonoBehaviour
{
    public static sond_manager instance;
    private AudioSource sond_effects;
    private AudioSource sond_background;

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
            sond_background.PlayOneShot(SoundTrack[musicIndex].soundClip);
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
        Debug.Log("sond_effects = " + sond_effects);
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