using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private int Load;
    private int MaxLoad;
    private int mora;
    List<CharacterData> CharactersOwnedList;
    List<Item> InventoryList;

    public void AddCharacterToOwnList(CharacterData characterData)
    {
        if (characterData == null)
            return;

        if (GetOwnedCharacterData(characterData.GetItemSO() as PlayerCharacterSO) == null) {
            CharactersOwnedList.Add(characterData);
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
        CharactersOwnedList = new List<CharacterData>();
    }

    public List<CharacterData> GetCharactersOwnedList()
    {
        return CharactersOwnedList;
    }
}
