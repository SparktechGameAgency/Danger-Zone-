using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{

    [Header("music")]
    [SerializeField] AudioSource background;   // already done
    [SerializeField] AudioSource SFX;  // new one for fail panel


    [Header("Music")]
    public AudioClip menuMusic;

    [Header("sfx")]
    public AudioClip win;
    public AudioClip lose;


    public void Start()
    {
        background.clip = menuMusic;
        background.Play();
    }

    public void playWinSFX()
    {
        SFX.clip = win;
        SFX.Play();
    }

    public void playLoseSFX()
    {
        SFX.clip = lose;
        SFX.Play();
    }

}
