using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public abstract class ConsumableItem : Item
{
    protected int amount;
    public event Action onValueChanged;

    public virtual void Use(int Useamount)
    {
        if (amount <= 0)
        {
            InventoryManager.GetInstance().RemoveItems(this);
            return;
        }

        amount = amount - Useamount;
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
        onValueChanged?.Invoke();
    }

    public ConsumableItem() : base()
    {
        amount = 1;
    }
}
