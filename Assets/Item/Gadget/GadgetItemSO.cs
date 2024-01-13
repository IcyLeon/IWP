using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetItemSO : ItemTemplate
{
    public override Type GetTypeREF()
    {
        return typeof(GadgetItem);
    }
    public override Category GetCategory()
    {
        return Category.GADGETS;
    }
    public override string GetItemType()
    {
        return "Gadget";
    }
}
