using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGadget : GadgetConsumableItem
{
    private Food CurrentFood;

    public FoodGadget(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
    }

    public void SetCurrentFood(Food food)
    {
        CurrentFood = food;
        amount = CurrentFood.GetAmount();
    }

    protected override void Use(int val)
    {
        if (CurrentFood != null)
            CurrentFood.Use(val);
    }
}
