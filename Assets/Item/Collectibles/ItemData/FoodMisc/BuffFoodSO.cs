using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffFoodData", menuName = "ScriptableObjects/BuffFoodSO")]
public class BuffFoodSO : FoodData
{
    [System.Serializable]
    public class StatsBoostInfo
    {
        public Artifacts.ArtifactsStat StatsBoost;
        public float boostValue;
    }

    public StatsBoostInfo[] StatsBoostInfoList;
    public float Duration;
    public override Type GetTypeREF()
    {
        return typeof(Food);
    }
}

