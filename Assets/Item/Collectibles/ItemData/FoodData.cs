using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "FoodData", menuName = "ScriptableObjects/FoodSO")]
public class FoodData : ItemTemplate
{
    public float Heal;

    public override Type GetType()
    {
        return typeof(Food);
    }

    public override Category GetCategory()
    {
        return Category.FOOD;
    }
    public override string GetItemType()
    {
        return "Food";
    }
}