using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItem : ConsumableItemForbiddenInInventory
{
    public float GetEXPAmount()
    {
        if (GetExpItemSO() == null)
            return 0f;

        return GetExpItemSO().ExpAmount;
    }

    public ExpItemSO GetExpItemSO()
    {
        return GetItemSO() as ExpItemSO; 
    }
    public ExpItem(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
    }
}
