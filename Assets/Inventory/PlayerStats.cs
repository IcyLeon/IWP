using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private int Load;
    private int MaxLoad;
    private int mora;
    List<Item> InventoryList;

    public int GetMora()
    {
        return mora;
    }

    public List<Item> GetINVList()
    {
        return InventoryList;
    }

    public void AddItems(Item item)
    {
        if (item == null)
            return;

        InventoryList.Add(item);
    }

    public void RemoveItems(Item item)
    {
        InventoryList.Remove(item);
    }

    public PlayerStats()
    {
        Load = 0;
        MaxLoad = 10000;
        mora = 0;
        InventoryList = new List<Item>();
    }
}
