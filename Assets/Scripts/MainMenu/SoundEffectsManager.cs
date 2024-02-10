using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsManager : MonoBehaviour
{
    public enum SoundMixerGroup
    {
        SOUND,
        MUSIC,
        VO
    }
    private static SoundEffectsManager instance;
    [SerializeField] ItemButtonSoundSO SoundSO;
    [SerializeField] AudioMixerGroup SoundSFXMixerGroup;
    [SerializeField] AudioMixerGroup MusicMixerGroup;
    [SerializeField] AudioMixerGroup VOMixerGroup;
    [SerializeField] AudioSource SoundSfxSource;
    [SerializeField] AudioSource VOSfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ItemButtonSoundSO GetSoundSO()
    {
        return SoundSO;
    }
    public void SetVolume(SoundMixerGroup SoundMixerGroup, float amt)
    {
        switch(SoundMixerGroup)
        {
            case SoundMixerGroup.SOUND:
                SoundSFXMixerGroup.audioMixer.SetFloat("SoundVolume", amt);
                break;
            case SoundMixerGroup.MUSIC:
                MusicMixerGroup.audioMixer.SetFloat("MusicVolume", amt);
                break;
            case SoundMixerGroup.VO:
                VOMixerGroup.audioMixer.SetFloat("VOVolume", amt);
                break;
        }
    }

    public static SoundEffectsManager GetInstance()
    {
        return instance;
    }

    public void PlayVOSound(AudioClip c, float spatialblend = 0f, Vector3 WorldPosition = default(Vector3), float volume = 1f)
    {
        //SoundEffectManager.PlaySound(c, spatialblend, WorldPosition, volume);

        VOSfxSource.spatialBlend = spatialblend;
        if (VOSfxSource.spatialBlend == 1)
        {
            VOSfxSource.transform.position = WorldPosition;
        }
        VOSfxSource.PlayOneShot(c, volume);

    }

    public void PlaySFXSound(AudioClip c, float spatialblend = 0f, Vector3 WorldPosition = default(Vector3), float volume = 1f)
    {
        SoundSfxSource.spatialBlend = spatialblend;
        if (SoundSfxSource.spatialBlend == 1)
        {
            SoundSfxSource.transform.position = WorldPosition;
        }
        SoundSfxSource.PlayOneShot(c, volume);

    }
}
