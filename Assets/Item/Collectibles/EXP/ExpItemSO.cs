using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExpItemSO")]
public class ExpItemSO : ItemTemplate
{
    public float ExpAmount;
    public override Type GetTypeREF()
    {
        return typeof(ExpItem);
    }

    public override string GetItemType()
    {
        return "Character EXP Material";
    }
}
