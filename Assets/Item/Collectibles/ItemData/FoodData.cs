using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "FoodData", menuName = "ScriptableObjects/FoodSO")]
public class FoodData : ItemTemplate
{
    public enum FoodType
    {
        BUFF,
        RESTORE_HEALTH,
    }
    [System.Serializable]
    public class StatsBoostInfo {
        public Artifacts.ArtifactsStat artifactsStat;
        public float boostValue;
    }

    [SerializeField] FoodType foodType; // Ignore if it is Revive Food
    public float InstantHeal;
    public StatsBoostInfo[] StatsBoostInfoList;
    public float Duration;

    public override Type GetTypeREF()
    {
        return typeof(Food);
    }

    public override Category GetCategory()
    {
        return Category.FOOD;
    }
    public override string GetItemType()
    {
        return GetFoodTypeString(foodType);
    }

    protected string GetFoodTypeString(FoodType f)
    {
        switch(f)
        {
            case FoodType.BUFF:
                return "ATK-Boosting Dishes";
            case FoodType.RESTORE_HEALTH:
                return "Recovery Dishes";
        }
        return "Unknown Dish";
    }
}