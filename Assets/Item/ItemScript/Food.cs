using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FoodData;

public class Food : ConsumableItem
{
    private float Timer;

    public void Update()
    {
        Timer -= Time.deltaTime;
    }

    public override void Use(int Useamount)
    {
        base.Use(Useamount);
    }

    public FoodData GetFoodData()
    {
        return GetItemSO() as FoodData;
    }

    public Food(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        Timer = GetFoodData().Duration;
    }
}
