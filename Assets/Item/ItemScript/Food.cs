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
    public Food(FoodData fd) : base()
    {
        category = Category.FOOD;
        Heal = fd.Heal;
    }
}
