using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum Elemental
{
    NONE,
    FIRE,
    ICE,
    ELECTRO,
    WIND
}

[CreateAssetMenu(fileName = "PlayersSO", menuName = "ScriptableObjects/PlayersSO")]
public class PlayerCharacterSO : CharactersSO
{
    public Elemental Elemental;
    public Sprite PartyIcon;

    [Header("Skills Info")]
    public Sprite ElementalSkillSprite;
    public Sprite ElementalBurstSprite;

    [Header("Skills Cooldown and Cost")]
    public float ElementalSkillsCooldown;
    public float UltiSkillCooldown;
    public float EnergyCost;
    [Header("Skills timer")]
    public float ElementalBurstTimer;
    public float ElementalSkillsTimer;

    [Header("Sound")]
    public AudioClip[] AttackVoiceList;
    public AudioClip[] ElementalSkillVoiceList;
    public AudioClip[] ElementalSkillRecastVoiceList;
    public AudioClip[] ElementalBurstVoiceList;
    public AudioClip[] FallenVoiceList;

    public override Type GetTypeREF()
    {
        return typeof(CharacterData);
    }

    public AudioClip GetRandomSkillVoice()
    {
        int index = Random.Range(0, ElementalSkillVoiceList.Length);
        return ElementalSkillVoiceList[index];
    }

    public AudioClip GetRandomSkillRecastVoice()
    {
        int index = Random.Range(0, ElementalSkillRecastVoiceList.Length);
        return ElementalSkillRecastVoiceList[index];
    }

    public AudioClip GetRandomFallenVoice()
    {
        int index = Random.Range(0, FallenVoiceList.Length);
        return FallenVoiceList[index];
    }
    public AudioClip GetRandomBasicAttackVoice()
    {
        int index = Random.Range(0, AttackVoiceList.Length);
        return AttackVoiceList[index];
    }
    public AudioClip GetRandomBurstVoice()
    {
        int index = Random.Range(0, ElementalBurstVoiceList.Length);
        return ElementalBurstVoiceList[index];
    }
}
