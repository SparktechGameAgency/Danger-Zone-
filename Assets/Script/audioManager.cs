using UnityEngine;

public class audioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] AudioSource background;
    [SerializeField] AudioSource SFX;

    [Header("Music Clips")]
    public AudioClip menuMusic;

    [Header("SFX Clips")]
    public AudioClip win;
    public AudioClip lose;

    void Start()
    {
        background.clip = menuMusic;
        background.loop = true;
        background.Play();
    }

    public void playWinSFX()
    {
        SFX.PlayOneShot(win);
    }

    public void playLoseSFX()
    {
        SFX.PlayOneShot(lose);
    }
}
