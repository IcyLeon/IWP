using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public abstract class Item {
    private bool FirstCreate;

    public bool isNew
    {
        get { return FirstCreate; }
        set { FirstCreate = value; }
    }
    protected Category category;
    public Category GetCategory
    {
        get { return category; }
        set { category = value; }
    }
    protected Rarity rarity;
    protected ItemTemplate ItemsSO;

    public void SetItemsSO(ItemTemplate itemsSO)
    {
        ItemsSO = itemsSO;
    }

    public ItemTemplate GetItemSO()
    {
        return ItemsSO;
    }


    public Rarity GetRarity()
    {
        return rarity;
    }

    public Item()
    {
        FirstCreate = true;
    }
}
