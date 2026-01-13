using UnityEngine;
using UnityEngine.UI;

public class MultiToggleButton : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite soundOnSprite;     // Image when sound is ON
    public Sprite soundOffSprite;    // Image when sound is OFF
    public Sprite vibrationOnSprite; // Image when vibration is ON
    public Sprite vibrationOffSprite;// Image when vibration is OFF
    public Sprite musicOnSprite;     // Image when music is ON
    public Sprite musicOffSprite;    // Image when music is OFF

    [Header("Components")]
    public Image soundButtonImage;   // UI Image for sound button
    public Image vibrationButtonImage; // UI Image for vibration button
    public Image musicButtonImage;   // UI Image for music button

    [Header("State")]
    public bool isSoundOn = true;    // Sound initially ON
    public bool isVibrationOn = true; // Vibration initially ON
    public bool isMusicOn = true;    // Music initially ON

    private void Start()
    {
        UpdateButtonImages();
    }

    // Call this from the Button OnClick()
    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;  // Toggle the sound state
        UpdateButtonImages();

        // OPTIONAL: Actually toggle sound (for example, using AudioListener or other audio settings)
        AudioListener.pause = !isSoundOn;  // Toggle global sound
    }

    // Call this from the Button OnClick()
    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;  // Toggle vibration state
        UpdateButtonImages();

        // OPTIONAL: Implement vibration control (if necessary)
        if (isVibrationOn)
        {
            // Example: Handheld.Vibrate(); // This is for mobile devices to trigger vibration
        }
    }

    // Call this from the Button OnClick()
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;  // Toggle music state
        UpdateButtonImages();

        // OPTIONAL: Actually toggle music (for example, using an AudioSource for background music)
        if (isMusicOn)
        {
            // Example: musicAudioSource.Play();
        }
        else
        {
            // Example: musicAudioSource.Pause();
        }
    }

    // Update button images based on the state of sound, vibration, and music
    private void UpdateButtonImages()
    {
        // Update sound button image
        soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;

        // Update vibration button image
        vibrationButtonImage.sprite = isVibrationOn ? vibrationOnSprite : vibrationOffSprite;

        // Update music button image
        musicButtonImage.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
    }
}
