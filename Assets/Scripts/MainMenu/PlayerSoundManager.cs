using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] AudioMixerGroup AudioMixerGroup;
    [SerializeField] int poolSize;
    private List<AudioSource> audioSourcePool;

    private void Start()
    {
        InitializeObjectPool();
    }

    private void InitializeObjectPool()
    {
        audioSourcePool = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject audioObject = new GameObject("AudioSourceObject");
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.transform.SetParent(transform);
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = AudioMixerGroup;
            audioSource.loop = false;
            audioObject.SetActive(false);
            audioSourcePool.Add(audioSource);
        }
    }

    public void PlaySound(AudioClip clip, float spatialblend = 0f, Vector3 WorldPosition = default(Vector3), float volume = 1f)
    {
        AudioSource availableAudioSource = GetAvailableAudioSource();

        if (availableAudioSource != null)
        {
            availableAudioSource.gameObject.SetActive(true);
            availableAudioSource.clip = clip;
            availableAudioSource.volume = volume;
            availableAudioSource.spatialBlend = spatialblend;
            if (availableAudioSource.spatialBlend == 1)
            {
                availableAudioSource.transform.position = WorldPosition;
            }
            availableAudioSource.Play();
            StartCoroutine(DeactivateAudioSourceAfterDelay_Coroutine(availableAudioSource));
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        return audioSourcePool.Find(audioSource => !audioSource.gameObject.activeSelf);
    }

    private IEnumerator DeactivateAudioSourceAfterDelay_Coroutine(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.gameObject.SetActive(false);
    }
}