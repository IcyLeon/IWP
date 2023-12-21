using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public override Type GetTypeREF()
    {
        return typeof(CharacterData);
    }
}
