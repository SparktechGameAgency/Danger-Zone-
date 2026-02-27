using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;  // background music only
    [SerializeField] private AudioSource sfxSource;    // click, win, lose SFX only

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("SFX Clips")]
    public AudioClip win;
    public AudioClip lose;
    public AudioClip click;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
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

    // ── Music ─────────────────────────────────────────────────────────────────

    public void PlayMenuMusic()
    {
        SwitchMusic(menuMusic);
    }

    public void PlayGameMusic()
    {
        SwitchMusic(gameMusic);
    }

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

        // Fade in — but only if music is not muted
        if (!musicSource.mute)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }
            musicSource.volume = 1;
        }
    }

    // ── Called by MultiToggleButton ───────────────────────────────────────────
    // Only mutes/unmutes the music source — sfxSource is NOT touched here
    public void SetMusicEnabled(bool enabled)
    {
        musicSource.mute = !enabled;

        // If turning music back on, restore full volume
        if (enabled)
            musicSource.volume = 1f;
    }

    // ── SFX ───────────────────────────────────────────────────────────────────
    // These use sfxSource which is never muted by the music toggle,
    // only silenced if AudioListener.pause is true (sound toggle OFF)

    public void PlayWinSFX()
    {
        sfxSource.PlayOneShot(win);
    }

    public void PlayLoseSFX()
    {
        sfxSource.PlayOneShot(lose);
    }

    public void PlayClickSFX()
    {
        sfxSource.PlayOneShot(click);
    }

    // ── Volume Controls ───────────────────────────────────────────────────────

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
}