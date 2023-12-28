using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private int Load;
    private int MaxLoad;
    private int coins;
    private int cash;
    List<CharacterData> CharactersOwnedList;
    List<Item> InventoryList;
    public delegate void onPlayerCurrencyChanged();
    public delegate void onPlayerCharactersAdd(CharacterData character);
    public onPlayerCharactersAdd OnPlayerCharactersAdd;
    public onPlayerCurrencyChanged OnCoinsChanged;
    public onPlayerCurrencyChanged OnCashChanged;

    public void AddCharacterToOwnList(CharacterData characterData)
    {
        if (characterData == null)
            return;

        if (GetOwnedCharacterData(characterData.GetItemSO() as PlayerCharacterSO) == null) {
            CharactersOwnedList.Add(characterData);
            OnPlayerCharactersAdd?.Invoke(characterData);
        }
    }

    public CharacterData GetOwnedCharacterData(PlayerCharacterSO playersSO)
    {
        for (int i = 0; i < CharactersOwnedList.Count; i++)
        {
            if (CharactersOwnedList[i].GetItemSO() == playersSO)
            {
                return CharactersOwnedList[i];
            }
        }
        return null;
    }
    public int CountNumberOfItems(ItemTemplate itemSOREF)
    {
        int total = 0;
        for (int i = 0; i < GetINVList().Count; i++)
        {
            Item item = GetINVList()[i];
            if (item.GetItemSO() == itemSOREF)
            {
                ConsumableItem cItem = item as ConsumableItem;
                if (cItem != null)
                {
                    total += cItem.GetAmount();
                }
                else
                {
                    total++;
                }
            }
        }
        return total;
    }
    public int GetCoins()
    {
        return coins;
    }

    public int GetCash()
    {
        return cash;
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

    public bool RemoveItems(Item item)
    {
        return InventoryList.Remove(item);
    }

    public PlayerStats()
    {
        Load = 0;
        MaxLoad = 10000;
        coins = 0;
        InventoryList = new List<Item>();
        CharactersOwnedList = new List<CharacterData>();
    }

    public List<CharacterData> GetCharactersOwnedList()
    {
        return CharactersOwnedList;
    }

    public void AddCoins(int val)
    {
        coins += val;
        OnCoinsChanged?.Invoke();
    }

    public void RemoveCoins(int val)
    {
        coins -= val;
        OnCoinsChanged?.Invoke();
    }

    public void AddCash(int val)
    {
        cash += val;
        OnCashChanged?.Invoke();
    }

    public void RemoveCash(int val)
    {
        cash -= val;
        OnCashChanged?.Invoke();
    }
}
