using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : Item
{
    protected int amount;

    public virtual void Use(int Useamount)
    {
        amount = amount - Useamount;
        if (amount <= 0)
        {
            InventoryManager.GetInstance().RemoveItems(this);
            return;
        }

        OverflowManager();
    }
    public int GetAmount()
    {
        return amount;
    }
    public void AddAmount(int amountAdd = 1)
    {
        amount += amountAdd;
        OverflowManager();
    }

    protected void OverflowManager()
    {
        amount = Mathf.Clamp(amount, 0, 1000);
        InventoryManager.GetInstance().CallOnInventoryChanged(this, ItemsSO);
    }


    public ConsumableItem(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        amount = 1;
    }
}
