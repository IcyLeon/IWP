using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemButtonSoundSO", menuName = "ScriptableObjects/ItemButtonSoundSO")]

public class ItemButtonSoundSO : ScriptableObject
{
    public AudioClip ArtifactSoundClick;
    public AudioClip DefaultSoundClick;
}
