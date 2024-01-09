using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetItem : Item
{
    public Action<GadgetItem> OnGadgetUse = delegate { };
    public GadgetItem(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
    }

    public virtual void UseGadget()
    {
        OnGadgetUse?.Invoke(this);
    }
}
