using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public AudioSource titleMusic;

    public List<AudioSource> BGM = new List<AudioSource>();

    public List<AudioSource> SFX = new List<AudioSource>();

    private bool bgmPlaying;
    private int currentTrack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bgmPlaying == true)
        {
            if (BGM[currentTrack].isPlaying == false)
            {
                StartBGM();
            }
        }
    }

    public void StopMusic()
    {
        titleMusic.Stop();

        foreach (AudioSource track in BGM)
        {
            track.Stop();
        }

        bgmPlaying = false;
    }

    public void StartTitleMusic()
    {
        StopMusic();

        titleMusic.Play();
    }

    public void StartBGM()
    {
        StopMusic();

        bgmPlaying = true;

        currentTrack = Random.Range(0, BGM.Count);

        BGM[currentTrack].Play();
    }

    public void PlaySFX(int SFXToPlay)
    {
        SFX[SFXToPlay].Stop();

        SFX[SFXToPlay].Play();
    }


}
