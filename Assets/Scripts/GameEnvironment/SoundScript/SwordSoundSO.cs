using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSoundSO", menuName = "ScriptableObjects/SwordSoundSO")]
public class SwordSoundSO : ScriptableObject
{
    [SerializeField] AudioClip[] HitClipList;
    [SerializeField] AudioClip[] SwingClipList;

    public AudioClip GetRandomHitClip()
    {
        if (HitClipList.Length == 0)
            return null;

        int randomSound = Random.Range(0, HitClipList.Length);
        return HitClipList[randomSound];
    }

    public AudioClip GetRandomSwingClip()
    {
        if (SwingClipList.Length == 0)
            return null;

        int randomSound = Random.Range(0, SwingClipList.Length);
        return SwingClipList[randomSound];
    }
}
