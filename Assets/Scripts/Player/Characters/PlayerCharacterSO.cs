using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float ElementalSkillsCooldown;
    public float UltiSkillCooldown;
    public float EnergyCost;
}
