using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GadgetItemSO", menuName = "ScriptableObjects/GadgetItemSO")]
public class FoodGadgetItemSO : GadgetItemSO
{
    public override Type GetTypeREF()
    {
        return typeof(FoodGadget);
    }
    public override Category GetCategory()
    {
        return Category.COLLECTIBLES;
    }
    public override string GetItemType()
    {
        return "Gadget";
    }
}
