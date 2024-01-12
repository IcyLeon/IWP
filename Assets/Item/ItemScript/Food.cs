using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : ConsumableItem
{
    private CharacterData characterData;

    public virtual void Update()
    {
    }


    public override void Use(int Useamount)
    {
        for (int i = 0; i < Useamount; i++)
        {
            if (amount > 0)
            {
                if (GetCharacterData() != null)
                {
                    ConsumeFood(this);
                }
            }
        }
        base.Use(Useamount);
    }

    protected virtual void ConsumeFood(Food food)
    {
        GetCharacterData().ConsumeFood(this);
    }

    public void SetCharacterData(CharacterData cd)
    {
        characterData = cd;
    }
    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    public FoodData GetFoodData()
    {
        return GetItemSO() as FoodData;
    }

    public Food(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
    }
}
