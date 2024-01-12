using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecoveryFoodData", menuName = "ScriptableObjects/RecoveryFoodSO")]
public class RecoveryFoodSO : FoodData
{
    public float FlatRestoreCharacterHP;
    public float RestoreCharacterHP_Percentage;
    public float AdditionalRecoveryHeal;

    public override Type GetTypeREF()
    {
        return typeof(RecoveryFood);
    }
}

