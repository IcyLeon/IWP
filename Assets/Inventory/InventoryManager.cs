using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        instance = this;
    }

    public static InventoryManager GetInstance()
    {
        return instance;
    }

    void Start()
    {   
        PlayerStats = new PlayerStats();
        EquipCharactersDatalist = new List<CharacterData>();

        for (int i = 0; i < startupSOTest.Length; i++)
        {
            CharacterData characterData = new CharacterData(startupSOTest[i]);
            AddCharacterDataToOwnList(characterData);
        }

        CharacterManager.GetInstance().SwapCharacters(0);

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
}
