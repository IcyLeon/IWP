using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReviveFoodData", menuName = "ScriptableObjects/ReviveFoodSO")]
public class ReviveFoodSO : FoodData
{
    public float ReviveCharacterHP;
    public float AdditionalRecoveryHeal;

    public override Type GetTypeREF()
    {
        return typeof(ReviveFood);
    }
}

