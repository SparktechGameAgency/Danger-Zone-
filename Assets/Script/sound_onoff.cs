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

    [Header("State")]
    public bool isSoundOn = true;
    public bool isVibrationOn = true;
    public bool isMusicOn = true;

    // Static so other scripts can check
    public static bool VibrationEnabled;

    private void Start()
    {
        VibrationEnabled = isVibrationOn; // initialize global state
        UpdateButtonImages();
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        UpdateButtonImages();
        AudioListener.pause = !isSoundOn;
    }

    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        VibrationEnabled = isVibrationOn;
        UpdateButtonImages();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        UpdateButtonImages();
    }

    private void UpdateButtonImages()
    {
        soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        vibrationButtonImage.sprite = isVibrationOn ? vibrationOnSprite : vibrationOffSprite;
        musicButtonImage.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
    }
}