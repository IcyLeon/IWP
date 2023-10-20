using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "ScriptableObjects/FoodSO")]
public class FoodData : ItemTemplate
{
    public float Heal;
    public override string GetItemType()
    {
        return "Food";
    }
}