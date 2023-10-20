using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersSO", menuName = "ScriptableObjects/PlayersSO")]
public class PlayersSO : ItemTemplate
{
    public float ElementalSkillsCooldown;
    public float UltiSkillCooldown;
    public float EnergyCost;
}
