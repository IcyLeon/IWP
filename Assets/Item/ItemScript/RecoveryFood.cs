using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryFood : Food
{
    protected CharacterManager characterManager;

    protected override void ConsumeFood(Food food)
    {
        base.ConsumeFood(food);

        RecoveryFoodSO rSO = food.GetItemSO() as RecoveryFoodSO;
        int TotalHeal = Mathf.RoundToInt(rSO.AdditionalRecoveryHeal + rSO.FlatRestoreCharacterHP + (rSO.RestoreCharacterHP_Percentage * 0.01f * GetCharacterData().GetActualMaxHealth(GetCharacterData().GetLevel())));
        characterManager.GetPlayerManager().HealCharacterBruteForce(GetCharacterData(), TotalHeal, food is not ReviveFood);
    }

    public RecoveryFood(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        characterManager = CharacterManager.GetInstance();
    }
}
