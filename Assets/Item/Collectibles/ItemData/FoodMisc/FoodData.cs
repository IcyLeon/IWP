using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FoodData : ItemTemplate
{
    public string[] FoodStatsInfo;

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
        return "Food";
    }
}