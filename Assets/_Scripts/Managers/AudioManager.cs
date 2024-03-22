using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> musicTracks = new List<AudioClip>();

    AudioSource musicSource;

    [SerializeField]
    AudioMixer audioMixer;

    int musicIndex;

    const string MIXER_MASTER = "MasterVolume";
    const string MIXER_SFX = "SFXVolume";
    const string MIXER_MUSIC = "MusicVolume";

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartMusic();
    }

    private void Update()
    {
        if(!musicSource.isPlaying && !PauseMenu.isPaused)
        {
            if (musicIndex + 1 > musicTracks.Count)
            {
                musicIndex = 0;
            }
            else
                musicIndex++;

            PlayTrack();
        }
    }

    private void OnEnable()
    {
        OptionsMenu.updateSettings += UpdateVolume;
        PauseMenu.onPaused += ToggleMusic;
        
    }

    private void OnDisable()
    {
        OptionsMenu.updateSettings -= UpdateVolume;
        PauseMenu.onPaused -= ToggleMusic;
    }

    void StartMusic()
    {
        int rand = Random.Range(0, musicTracks.Count);
        musicIndex = rand;
        musicSource.clip = musicTracks[musicIndex];
        musicSource.PlayDelayed(5);
    }

    void PlayTrack()
    {
        Debug.Log("Music Index: " + musicIndex);
        musicSource.clip = musicTracks[musicIndex];
        musicSource.Play();
    }

    void ToggleMusic(bool isPaused)
    {
        if (isPaused)
            PauseMusic();
        else
            ResumeMusic();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    void UpdateVolume(PlayerSettings updatedSettings)
    {
        audioMixer.SetFloat(MIXER_MASTER, Mathf.Log10(updatedSettings.masterVolume.currentValue) * 20);
        audioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(updatedSettings.musicVolume.currentValue) * 20);
        audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(updatedSettings.sfxVolume.currentValue) * 20);
    }
}
