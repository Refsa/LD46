using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;

    [SerializeField] AudioSource pickupAudioSource;

    [SerializeField] AudioClip fuseBurnBeep;
    [SerializeField] AudioClip burnRateBeep;
    [SerializeField] AudioClip gameOverSound;
    [SerializeField] AudioClip buffPickupSound;
    [SerializeField] AudioClip debuffPickupSound;

    AudioSource audioSource;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);   

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayFuseBrunBeep()
    {
        PlayOneShot(instance.fuseBurnBeep);
    }

    public static void PlayBurnRateBeep()
    {
        PlayOneShot(instance.burnRateBeep);
    }

    public static void PlayBuffPickupSound()
    {
        instance.pickupAudioSource.PlayOneShot(instance.buffPickupSound);
    }

    public static void PlayDebuffPickupSound()
    {
        instance.pickupAudioSource.PlayOneShot(instance.debuffPickupSound);
    }

    public static void PlayGameOverSound()
    {
        instance.pickupAudioSource.PlayOneShot(instance.gameOverSound, 1f);
    }

    public static void PlayOneShot(AudioClip clip)
    {
        instance.audioSource.PlayOneShot(clip);
    }
}
