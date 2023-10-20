using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    private static InventoryManager instance;
    private PlayerStats PlayerStats;
    private List<Characters> equipcharacterslist;
    private Characters currentequipCharacter;

    public delegate void OnInventoryListChanged();
    public OnInventoryListChanged onInventoryListChanged;

    private void Awake()
    {
        instance = this;
    }

    public static InventoryManager GetInstance()
    {
        return instance;
    }

    public InventoryManager()
    {
        PlayerStats = new PlayerStats();
        equipcharacterslist = new List<Characters>();
    }

    public PlayerStats GetPlayerStats()
    {
        return PlayerStats;
    }

    public List<Item> GetINVList()
    {
        if (PlayerStats == null)
            return null;

        return PlayerStats.GetINVList();
    }
    public void AddItems(Item item)
    {
        if (PlayerStats == null)
            return;

        if (isConsumableItemExisted(item))
        {
            ConsumableItem consumableItem = item as ConsumableItem;
            consumableItem.AddAmount();
            return;
        }

        PlayerStats.AddItems(item);
        onInventoryListChanged?.Invoke();
    }

    private bool isConsumableItemExisted(Item item)
    {
        ConsumableItem consumableItem = item as ConsumableItem;
        foreach (Item inventoryItem in PlayerStats.GetINVList())
        {
            ConsumableItem inventoryConsumableItem = inventoryItem as ConsumableItem;
            if (inventoryConsumableItem != null && inventoryConsumableItem.Equals(consumableItem))
            {
                return true;
            }
        }


        return false;
    }

    public void RemoveItems(Item item)
    {
        PlayerStats.RemoveItems(item);
        onInventoryListChanged?.Invoke();
    }
    public List<Characters> GetEquipCharactersList()
    {
        return equipcharacterslist;
    }

    public Characters GetCurrentEquipCharacter()
    {
        return currentequipCharacter;
    }

    public void SetCurrentEquipCharacter(Characters characters)
    {
        currentequipCharacter = characters;
    }
}
