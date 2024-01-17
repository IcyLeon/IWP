using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    private static InventoryManager instance;
    private PlayerStats PlayerStats;
    private List<CharacterData> EquipCharactersDatalist;
    private CharacterData currentequipCharacter;
    private GadgetItem currentEquipGadgetItem;

    public delegate void OnInventoryListChange(Item item, ItemTemplate itemSO);
    public static OnInventoryListChange OnInventoryItemAdd;
    public static OnInventoryListChange OnInventoryItemRemove;
    public static OnInventoryListChange OnInventoryChanged; // if there is a change in the inventory (be it add or remove for all types of item)
    public static Action<List<Item>> OnItemGiveToPlayer = delegate { };
    
    [SerializeField] PlayerCharacterSO[] startupSOTest;
    [SerializeField] GadgetItemSO GadgetItemSO;

    public static void OnCallGivePlayerItems(List<Item> ItemList)
    {
        OnItemGiveToPlayer?.Invoke(ItemList);
    }

    public static Item CreateItem(ItemTemplate itemSO)
    {
        Type ItemType = itemSO.GetTypeREF();
        object instance = Activator.CreateInstance(ItemType, true, itemSO);
        Item item = (Item)instance;
        return item;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerStats = new PlayerStats();
        EquipCharactersDatalist = new List<CharacterData>();
        FallenUI.OnReviveChange += ResetEverything;
    }

    private void OnDestroy()
    {
        FallenUI.OnReviveChange -= ResetEverything;
    }


    void Start()
    {
        for (int i = 0; i < startupSOTest.Length; i++)
        {
            CharacterData characterData = new CharacterData(true, startupSOTest[i]);
            AddCharacterDataToOwnList(characterData);
        }
        SetCurrentEquipCharacter(GetCharactersOwnedList()[0]); // change this


        InitDefaultItemInInventory();
    }

    void InitDefaultItemInInventory()
    {
        GadgetItem item = CreateItem(GadgetItemSO) as GadgetItem;
        AddItems(item);
        SetCurrentGadgetItemEquipped(item);
    }
    public void SetCurrentGadgetItemEquipped(GadgetItem gadgetItem)
    {
        currentEquipGadgetItem = gadgetItem;
    }

    public GadgetItem GetCurrentGadgetItem()
    {
        return currentEquipGadgetItem;
    }

    public static InventoryManager GetInstance()
    {
        return instance;
    }

    public List<CharacterData> GetCharactersOwnedList()
    {
        if (PlayerStats == null)
            return null;

        return PlayerStats.GetCharactersOwnedList();
    }
    public CharacterData GetOwnedCharacterData(PlayerCharacterSO playersSO)
    {
        if (PlayerStats == null)
            return null;

        return PlayerStats.GetOwnedCharacterData(playersSO);
    }

    private void AddCharacterDataToOwnList(CharacterData characterData)
    {
        if (PlayerStats == null)
            return;

        PlayerStats.AddCharacterToOwnList(characterData);
    }

    public PlayerStats GetPlayerStats()
    {
        return PlayerStats;
    }

    public Item GetItemREF(ItemTemplate itemsSO)
    {
        for (int i = 0; i < GetPlayerStats().GetINVList().Count; i++)
        {
            Item item = GetPlayerStats().GetINVList()[i];
            if (item.GetItemSO() == itemsSO)
                return item;
        }
        return null;
    }
    public int CountNumberOfItems(ItemTemplate itemSOREF)
    {
        if (GetPlayerStats() == null)
            return 0;

        return GetPlayerStats().CountNumberOfItems(itemSOREF);
    }

    public List<T> GetItemListOfType<T>()
    {
        List<T> tempList = new();

        if (PlayerStats == null)
            return null;

        foreach (var item in GetINVList())
        {
            if (item is T typedItem)
            {
                tempList.Add(typedItem);
            }
        }
        return tempList;
    }

    public List<Item> GetINVList()
    {
        if (PlayerStats == null)
            return null;

        return PlayerStats.GetINVList();
    }
    public Item AddItems(Item item)
    {
        if (PlayerStats == null)
            return null;

        Item ExistItem = isConsumableItemExisted(item);
        if (ExistItem != null)
        {
            ConsumableItem consumableItem = ExistItem as ConsumableItem;
            consumableItem.AddAmount();
            CallOnInventoryChanged(consumableItem, consumableItem.GetItemSO());
            return consumableItem;
        }

        PlayerStats.AddItems(item);
        OnInventoryItemAdd?.Invoke(item, item.GetItemSO());
        CallOnInventoryChanged(item, item.GetItemSO());
        return item;
    }

    public void CallOnInventoryChanged(Item item, ItemTemplate itemSO)
    {
        OnInventoryChanged?.Invoke(item, itemSO);
    }

    private Item isConsumableItemExisted(Item ExistItemCheck)
    {
        ConsumableItem consumableItem = ExistItemCheck as ConsumableItem;
        if (consumableItem == null)
            return null;

        foreach (Item inventoryItem in PlayerStats.GetINVList())
        {
            if (inventoryItem.GetItemSO() == consumableItem.GetItemSO())
            {
                return inventoryItem;
            }
        }


        return null;
    }

    public void RemoveItems(Item item)
    {
        if (PlayerStats == null)
            return;

        if (PlayerStats.RemoveItems(item))
        {
            OnInventoryItemRemove?.Invoke(item, item.GetItemSO());
            CallOnInventoryChanged(item, item.GetItemSO());
        }
    }

    public void ResetEverything()
    {
        if (PlayerStats == null)
            return;

        for (int i = GetINVList().Count - 1; i >= 0; i--)
        {
            RemoveItems(GetINVList()[i]);
        }
        PlayerStats.ResetCharactersLevels();

        InitDefaultItemInInventory();
    }

    public List<CharacterData> GetEquipCharactersDataList()
    {
        return EquipCharactersDatalist;
    }

    public CharacterData GetCurrentEquipCharacterData()
    {
        return currentequipCharacter;
    }

    public void SetCurrentEquipCharacter(CharacterData CharacterData)
    {
        currentequipCharacter = CharacterData;
    }

    public void AddCurrency(CurrencyType type, int amt)
    {
        switch (type)
        {
            case CurrencyType.COINS:
                GetPlayerStats().AddCoins(amt);
                break;
            case CurrencyType.CASH:
                GetPlayerStats().AddCash(amt);
                break;
        }
    }

    public void RemoveCurrency(CurrencyType type, int amt)
    {
        switch (type)
        {
            case CurrencyType.COINS:
                GetPlayerStats().RemoveCoins(amt);
                break;
            case CurrencyType.CASH:
                GetPlayerStats().RemoveCash(amt);
                break;
        }
    }

    public int GetCurrency(CurrencyType type)
    {
        switch(type)
        {
            case CurrencyType.COINS:
                return GetPlayerStats().GetCoins();
            case CurrencyType.CASH:
                return GetPlayerStats().GetCash();
        }
        return 0;
    }
}
