using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootStepSO", menuName = "ScriptableObjects/FootStepSO")]
public class FootStepSO : ScriptableObject
{
    public AudioClip[] FootstepClip;

    public AudioClip GetRandomFootstepSound()
    {
        int index = Random.Range(0, FootstepClip.Length);
        return FootstepClip[index];
    }
}
