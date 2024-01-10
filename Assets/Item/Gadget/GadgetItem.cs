using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetItem : Item
{
    public GadgetItem(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
    }

    public virtual void UseGadget()
    {
    }

    public virtual void Update()
    {

    }
}
