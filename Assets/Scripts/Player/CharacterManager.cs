using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class CharacterInfo
{
    public GameObject CharacterPrefab;
    public PlayerCharacterSO playersSO;
}

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager instance;
    private PlayerController playerController;
    [SerializeField] List<CharacterInfo> charactersInfo = new();
    List<PlayerCharacters> PlayerCharactersList = new();
    private PlayerCharacters CurrentCharacter;
    public delegate void OnCharacterChange(CharacterData characterData);
    public OnCharacterChange onCharacterChange;
    private InventoryManager inventoryManager;
    private ElementsIndicator elementsIndicator;

    private void Awake()
    {
        instance = this;
    }

    public void AddPlayerCharactersList(PlayerCharacters pc)
    {
        PlayerCharactersList.Add(pc);
    }

    public ElementsIndicator GetElementsIndicator()
    {
        return elementsIndicator;
    }

    public void SetElementsIndicator(ElementsIndicator ElementsIndicator)
    {
        elementsIndicator = ElementsIndicator;
    }


    private void Start()
    {
        if (playerController)
            playerController.OnNumsKeyInput += SwapCharactersControls;

        SwapCharacters(0);
    }

    private void Update()
    {
        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        for (int i = 0; i < inventoryManager.GetCharactersOwnedList().Count; i++)
        {
            CharacterData characterData = inventoryManager.GetCharactersOwnedList()[i];
            characterData.Update();
        }
    }

    private void SwapCharactersControls(int index)
    {
        SwapCharacters(index - 1);
    }

    public void SwapCharacters(int index)
    {
        inventoryManager = InventoryManager.GetInstance();
        if (index >= inventoryManager.GetCharactersOwnedList().Count)
            return;

        PlayerCharacters playerCharacters = GetPlayerCharacter(inventoryManager.GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);
        CharacterData characterData = inventoryManager.GetOwnedCharacterData(inventoryManager.GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);

        if (characterData != null && playerCharacters != null)
        {
            for (int i = 0; i < PlayerCharactersList.Count; i++)
            {
                PlayerCharactersList[i].gameObject.SetActive(false);
            }

            playerCharacters.SetCharacterData(characterData);
            inventoryManager.SetCurrentEquipCharacter(characterData);
            SetCurrentCharacter(playerCharacters);
            playerCharacters.gameObject.SetActive(true);

            if (GetElementsIndicator() != null)
            {
                Destroy(GetElementsIndicator().gameObject);
            }
            onCharacterChange?.Invoke(CurrentCharacter.GetCharacterData());
        }

    }

    public void SetCurrentCharacter(PlayerCharacters character)
    {
        CurrentCharacter = character;
    }

    public PlayerCharacters GetCurrentCharacter()
    {
        return CurrentCharacter;
    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    private PlayerCharacters GetPlayerCharacter(PlayerCharacterSO playersSO)
    {
        for (int i = 0; i < PlayerCharactersList.Count; i++)
        {
            if (PlayerCharactersList[i].GetPlayersSO() == playersSO)
                return PlayerCharactersList[i];
        }
        return null;
    }

    public CharacterInfo GetCharacterInfo(PlayerCharacterSO playersSO)
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

    public List<CharacterData> GetCharactersOwnedList()
    {
        return InventoryManager.GetInstance().GetCharactersOwnedList();
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
