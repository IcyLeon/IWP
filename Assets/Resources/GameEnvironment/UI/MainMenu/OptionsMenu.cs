using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider SoundSfxSlider;
    [SerializeField] Slider MusicSfxSlider;
    [SerializeField] Slider DialogueSfxSlider;

    [Header("SFX")]
    [SerializeField] AudioMixerGroup Music;
    [SerializeField] AudioMixerGroup VO;
    [SerializeField] AudioMixerGroup Sound;

    // Start is called before the first frame update
    void Start()
    {
        SoundSfxSlider.onValueChanged.AddListener(SoundChange);
        MusicSfxSlider.onValueChanged.AddListener(MusicChange);
        DialogueSfxSlider.onValueChanged.AddListener(DialogueChange);

        SoundSfxSlider.minValue = MusicSfxSlider.minValue = DialogueSfxSlider.minValue = -80f;
        SoundSfxSlider.maxValue = MusicSfxSlider.maxValue = DialogueSfxSlider.maxValue = 0f;
        SoundSfxSlider.value = SoundSfxSlider.maxValue;
        MusicSfxSlider.value = MusicSfxSlider.maxValue;
        DialogueSfxSlider.value = DialogueSfxSlider.maxValue;

    }

    private void SoundChange(float val)
    {
        Sound.audioMixer.SetFloat("SFXVolume", val);
    }

    private void DialogueChange(float val)
    {
        VO.audioMixer.SetFloat("VOVolume", val);
    }
    private void MusicChange(float val)
    {
        Music.audioMixer.SetFloat("MusicVolume", val);
    }
}
