using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Food : ConsumableItem
{
    private float Heal;

    public override void Use(int Useamount)
    {
        base.Use(Useamount);
    }
    public Food(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        FoodData fd = GetItemSO() as FoodData;
        Heal = fd.Heal;
    }
}
