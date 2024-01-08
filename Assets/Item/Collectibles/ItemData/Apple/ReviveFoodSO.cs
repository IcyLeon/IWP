using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveFoodSO : FoodData
{
    public float ReviveCharacterHP;
    public float AdditionalRecoveryHeal;

    public override string GetItemType()
    {
        return "Recovery Dishes";
    }
}
