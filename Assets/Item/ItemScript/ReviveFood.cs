using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveFood : Food
{
    private CharacterManager characterManager;
    protected override void ConsumeFood(Food food)
    {
        base.ConsumeFood(food);

        ReviveFoodSO rSO = food.GetItemSO() as ReviveFoodSO;
        int TotalHeal = Mathf.RoundToInt(rSO.AdditionalRecoveryHeal + rSO.ReviveCharacterHP);
        characterManager.GetPlayerManager().HealCharacterBruteForce(GetCharacterData(), TotalHeal, false);
    }

    public ReviveFood(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        characterManager = CharacterManager.GetInstance();
    }
}
