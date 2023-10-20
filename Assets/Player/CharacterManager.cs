using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class CharacterInfo
{
    public GameObject CharacterPrefab;
    public PlayersSO playersSO;
}

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager instance;
    private PlayerController playerController;
    [SerializeField] List<CharacterInfo> charactersInfo = new List<CharacterInfo>();
    private Characters CurrentCharacter;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        if (playerController)
            playerController.onNumsKeyInput += SwapCharactersControls;
    }

    private void SwapCharactersControls(int index)
    {
        SwapCharacters(index - 1);
    }
    public void SwapCharacters(int index)
    {
        InventoryManager inventoryManager = InventoryManager.GetInstance();
        if (index >= inventoryManager.GetCharactersOwnedList().Count)
            return;

        CharacterInfo characterInfo = GetCharacterInfo(inventoryManager.GetCharactersOwnedList()[index].GetItemSO() as PlayersSO);
        CharacterData characterData = inventoryManager.GetOwnedCharacterData(inventoryManager.GetCharactersOwnedList()[index].GetItemSO() as PlayersSO);

        if (characterData != null && characterInfo != null)
        {
            Characters CurrentCharacter = GetCurrentCharacter();
            if (CurrentCharacter != null)
            {
                Destroy(CurrentCharacter.gameObject);
                SetCurrentCharacter(null);
            }
            CurrentCharacter = Instantiate(characterInfo.CharacterPrefab, playerController.transform).GetComponent<Characters>();
            CurrentCharacter.SetCharacterData(characterData);
            inventoryManager.SetCurrentEquipCharacter(characterData);
            SetCurrentCharacter(CurrentCharacter);
        }

    }

    public void SetCurrentCharacter(Characters character)
    {
        CurrentCharacter = character;
    }

    public Characters GetCurrentCharacter()
    {
        return CurrentCharacter;
    }


    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public CharacterInfo GetCharacterInfo(PlayersSO playersSO)
    {
        for (int i = 0; i < charactersInfo.Count; i++)
        {
            if (charactersInfo[i].playersSO == playersSO)
                return charactersInfo[i];
        }
        return null;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }
    public List<CharacterData> GetEquippedCharacterData()
    {
        return InventoryManager.GetInstance().GetEquipCharactersDataList();
    }
    public List<CharacterInfo> GetCharacterList()
    {
        return charactersInfo;
    }

    public static CharacterManager GetInstance()
    {
        return instance;
    }
}
