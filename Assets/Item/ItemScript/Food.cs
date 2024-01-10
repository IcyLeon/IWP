using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : ConsumableItem
{
    private float Timer;
    private CharacterData characterData;

    public void Update()
    {
        if (Timer > 0)
            Timer -= Time.deltaTime;
    }

    public bool isFoodBuffEnds()
    {
        return Timer <= 0f;
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
                base.Use(Useamount);
            }
        }
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
        Timer = GetFoodData().Duration;
    }
}
