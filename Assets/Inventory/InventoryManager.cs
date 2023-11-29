using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryManager;

public class InventoryManager : MonoBehaviour {
    private static InventoryManager instance;
    private PlayerStats PlayerStats;
    private List<CharacterData> EquipCharactersDatalist;
    private CharacterData currentequipCharacter;

    public delegate void OnInventoryListChanged();
    public OnInventoryListChanged onInventoryListChanged;

    [SerializeField] PlayerCharacterSO[] startupSOTest;

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
    }

    public static InventoryManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {   
        for (int i = 0; i < startupSOTest.Length; i++)
        {
            CharacterData characterData = new CharacterData(true, startupSOTest[i]);
            AddCharacterDataToOwnList(characterData);
        }
        SpawnCharacters();
    }

    public void SpawnCharacters()
    {
        for (int i = 0; i < GetCharactersOwnedList().Count; i++)
        {
            CharacterInfo characterInfo = CharacterManager.GetInstance().GetCharacterInfo(GetCharactersOwnedList()[i].GetItemSO() as PlayerCharacterSO);
            PlayerCharacters CurrentCharacter = Instantiate(characterInfo.CharacterPrefab, CharacterManager.GetInstance().GetPlayerController().transform).GetComponent<PlayerCharacters>();
            CurrentCharacter.SetCharacterData(GetCharactersOwnedList()[i]);
            CharacterManager.GetInstance().AddPlayerCharactersList(CurrentCharacter);
        }
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
            return consumableItem;
        }

        PlayerStats.AddItems(item);
        onInventoryListChanged?.Invoke();
        return item;
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
        PlayerStats.RemoveItems(item);
        onInventoryListChanged?.Invoke();
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
