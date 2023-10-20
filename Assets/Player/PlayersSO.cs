using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersSO", menuName = "ScriptableObjects/PlayersSO")]
public class PlayersSO : ScriptableObject
{
    public string CharacterName;
    public string CharacterDesc;
    public Sprite CharacterSprites;
    public float ElementalSkillsCooldown;
    public float UltiSkillCooldown;
}
