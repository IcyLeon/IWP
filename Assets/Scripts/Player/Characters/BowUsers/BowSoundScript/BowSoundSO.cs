using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BowSoundSO", menuName = "ScriptableObjects/BowSoundSO")]
public class BowSoundSO : ScriptableObject
{
    public AudioClip BowChargeUpAudioClip;
    public AudioClip[] BasicFireAudioClip;
    public AudioClip AimFireAudioClip;

    public AudioClip GetRandomBasicFireAudioClip()
    {
        int random = Random.Range(0, BasicFireAudioClip.Length);
        return BasicFireAudioClip[random];
    }
}
