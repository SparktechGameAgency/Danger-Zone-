using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("SFX Clips")]
    public AudioClip win;
    public AudioClip lose;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    // 🎵 MENU MUSIC
    public void PlayMenuMusic()
    {
        SwitchMusic(menuMusic);
    }

    // 🎮 GAME MUSIC
    public void PlayGameMusic()
    {
        SwitchMusic(gameMusic);
    }

    // 🎶 Music Switching with Fade
    private void SwitchMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeMusic(newClip));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 1;
    }

    // 🔊 SFX
    public void PlayWinSFX()
    {
        sfxSource.PlayOneShot(win);
    }

    public void PlayLoseSFX()
    {
        sfxSource.PlayOneShot(lose);
    }

    // 🎚 Volume Controls
    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void ToggleMute(bool isMuted)
    {
        musicSource.mute = isMuted;
        sfxSource.mute = isMuted;
    }
}