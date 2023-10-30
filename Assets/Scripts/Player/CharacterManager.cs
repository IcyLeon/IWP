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
    [SerializeField] List<CharacterInfo> charactersInfo = new List<CharacterInfo>();
    private PlayerCharacters CurrentCharacter;
    public delegate void OnCharacterChange(CharacterData characterData);
    public OnCharacterChange onCharacterChange;
    private InventoryManager inventoryManager;
    private ElementsIndicator elementsIndicator;

    private void Awake()
    {
        instance = this;
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

        CharacterInfo characterInfo = GetCharacterInfo(inventoryManager.GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);
        CharacterData characterData = inventoryManager.GetOwnedCharacterData(inventoryManager.GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);

        if (characterData != null && characterInfo != null)
        {
            PlayerCharacters CurrentCharacter = GetCurrentCharacter();
            if (CurrentCharacter != null)
            {
                Destroy(CurrentCharacter.gameObject);
                SetCurrentCharacter(null);
            }

            CurrentCharacter = Instantiate(characterInfo.CharacterPrefab, playerController.transform).GetComponent<PlayerCharacters>();
            CurrentCharacter.SetCharacterData(characterData);
            inventoryManager.SetCurrentEquipCharacter(characterData);
            SetCurrentCharacter(CurrentCharacter);

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
