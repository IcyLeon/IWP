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
            audioObject.SetActive(false);
            audioSourcePool.Add(audioSource);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource availableAudioSource = GetAvailableAudioSource();

        if (availableAudioSource != null)
        {
            availableAudioSource.gameObject.SetActive(true);
            availableAudioSource.clip = clip;
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