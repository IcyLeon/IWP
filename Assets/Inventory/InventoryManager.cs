using System;
using System.Collections;
using System.Collections.Generic;
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
            CharacterData characterData = new CharacterData(startupSOTest[i]);
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

    public void AddMora(int mora)
    {
        if (GetPlayerStats() == null)
            return;

        GetPlayerStats().AddMora(mora);
    }

    public void RemoveMora(int mora)
    {
        if (GetPlayerStats() == null)
            return;

        GetPlayerStats().RemoveMora(mora);
    }

    public int GetCoins()
    {
        if (GetPlayerStats() == null)
            return 0;

        return GetPlayerStats().GetMora();
    }
}
