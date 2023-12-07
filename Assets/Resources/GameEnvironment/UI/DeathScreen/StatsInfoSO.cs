using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatsInfoType
{
    ENEMY_KILL,
    TOTAL_COINS,
    TOTAL_CASH,
}

[CreateAssetMenu(fileName = "StatsInfoSO", menuName = "ScriptableObjects/StatsInfoSO")]
public class StatsInfoSO : ScriptableObject
{
    public StatsInfoType statsInfoType;
    public string StatsName;

}
