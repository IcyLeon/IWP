using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider SoundSfxSlider;
    [SerializeField] Slider MusicSfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        SoundSfxSlider.onValueChanged.AddListener(SoundChange);
        MusicSfxSlider.onValueChanged.AddListener(MusicChange);
    }

    private void SoundChange(float val)
    {
    }

    private void MusicChange(float val)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
