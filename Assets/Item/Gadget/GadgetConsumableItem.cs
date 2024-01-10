using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetConsumableItem : GadgetItem
{
    protected int amount;

    public override void UseGadget()
    {
        Use(1);
    }

    protected virtual void Use(int val)
    {
        if (amount > 0)
        {
            amount -= val;
        }
        amount = Mathf.Max(0, amount);
    }

    public int GetAmount()
    {
        return amount;
    }
    public GadgetConsumableItem(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
    }
}
