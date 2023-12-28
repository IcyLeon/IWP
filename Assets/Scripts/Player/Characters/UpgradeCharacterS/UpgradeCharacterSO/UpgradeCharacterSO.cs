using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpgradeCharacterSO")]
public class UpgradeCharacterSO : ScriptableObject
{
    public int[] CharacterUpgradeCost;
}
