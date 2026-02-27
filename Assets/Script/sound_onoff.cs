using UnityEngine;
using UnityEngine.UI;

public class MultiToggleButton : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Sprite vibrationOnSprite;
    public Sprite vibrationOffSprite;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    [Header("Components")]
    public Image soundButtonImage;
    public Image vibrationButtonImage;
    public Image musicButtonImage;

    // Static so DangerZoneManager can check vibration anywhere
    public static bool VibrationEnabled;

    // PlayerPrefs keys
    private const string PREF_SOUND = "SoundOn";
    private const string PREF_MUSIC = "MusicOn";
    private const string PREF_VIBRATION = "VibrationOn";

    private bool isSoundOn;
    private bool isMusicOn;
    private bool isVibrationOn;

    private void Start()
    {
        // Load saved preferences (default all ON)
        isSoundOn = PlayerPrefs.GetInt(PREF_SOUND, 1) == 1;
        isMusicOn = PlayerPrefs.GetInt(PREF_MUSIC, 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt(PREF_VIBRATION, 1) == 1;

        VibrationEnabled = isVibrationOn;

        ApplySound();
        ApplyMusic();
        UpdateButtonImages();
    }

    // ── SOUND ─────────────────────────────────────────────────────────────────
    // Turns OFF everything — music + all SFX (click, win, lose)
    // Uses AudioListener.pause which silences the entire game globally
    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt(PREF_SOUND, isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplySound();
        UpdateButtonImages();
    }

    // ── MUSIC ─────────────────────────────────────────────────────────────────
    // Only mutes the background music source (menu music + game music)
    // SFX (click, win, lose) keep playing normally
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt(PREF_MUSIC, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMusic();
        UpdateButtonImages();
    }

    // ── VIBRATION ─────────────────────────────────────────────────────────────
    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        VibrationEnabled = isVibrationOn;
        PlayerPrefs.SetInt(PREF_VIBRATION, isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateButtonImages();
    }

    // ── Apply ─────────────────────────────────────────────────────────────────

    private void ApplySound()
    {
        // Pausing AudioListener silences ALL audio in the scene
        AudioListener.pause = !isSoundOn;
    }

    private void ApplyMusic()
    {
        if (audioManager.Instance != null)
            audioManager.Instance.SetMusicEnabled(isMusicOn);
    }

    // ── Visuals ───────────────────────────────────────────────────────────────

    private void UpdateButtonImages()
    {
        soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        musicButtonImage.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
        vibrationButtonImage.sprite = isVibrationOn ? vibrationOnSprite : vibrationOffSprite;
    }
}