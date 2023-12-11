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
    private List<PlayerCharacters> PlayerCharactersList = new();
    private PlayerCharacters CurrentCharacter;
    public delegate void OnCharacterChange(CharacterData characterData);
    public OnCharacterChange onCharacterChange;
    private InventoryManager inventoryManager;
    private ElementsIndicator elementsIndicator;
    private CharacterData CurrentSelectionCharacterData;

    private SceneManager SceneManager;

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
    }
    
    public void AddPlayerCharactersList(PlayerCharacters pc)
    {
        PlayerCharactersList.Add(pc);
    }

    public ElementsIndicator GetElementsIndicator()
    {
        return elementsIndicator;
    }

    public void AddAllCharactersPercentage(float percentage)
    {
        for (int i = 0; i < GetCharactersOwnedList().Count; i++)
        {
            PlayerCharactersList[i].SetHealth(PlayerCharactersList[i].GetHealth() + PlayerCharactersList[i].GetMaxHealth() * percentage);
        }
    }

    public void HealCharacterBruteForce(CharacterData c, float amt)
    {
        if (c == null)
            return;

        float actualamt = amt;
        if (actualamt < 1f)
            actualamt = 1f;

        c.SetHealth(c.GetHealth() + (int)actualamt);
        if (c.GetHealth() < c.GetMaxHealth())
            AssetManager.GetInstance().SpawnWorldText_Other(GetPlayerController().GetPlayerOffsetPosition().position, OthersState.HEAL, "+" + (int)actualamt);
    }

    public void HealCharacter(CharacterData c, float amt)
    {
        if (c == null)
            return;

        if (c.IsDead())
            return;

            float actualamt = amt;
        if (actualamt < 1f)
            actualamt = 1f;

        c.SetHealth(c.GetHealth() + (int)actualamt);
        if (c.GetHealth() < c.GetMaxHealth())
        {
            AssetManager.GetInstance().SpawnWorldText_Other(GetPlayerController().GetPlayerOffsetPosition().position, OthersState.HEAL, "+" + (int)actualamt);
            if (c == CurrentSelectionCharacterData)
            {
                ParticleSystem healEffect = Instantiate(AssetManager.GetInstance().HealEffect, GetPlayerController().transform).GetComponent<ParticleSystem>();
                Destroy(healEffect.gameObject, healEffect.main.duration);
            }
        }

    }

    public CharacterData GetAliveCharacters()
    {
        List<PlayerCharacters> PlayerCharacerListCopy = new(PlayerCharactersList);
        for (int i = PlayerCharacerListCopy.Count - 1; i >= 0; i--)
        {
            if (PlayerCharacerListCopy[i].IsDead())
            {
                PlayerCharacerListCopy.RemoveAt(i);
            }
        }

        if (PlayerCharacerListCopy.Count > 0)
            return PlayerCharacerListCopy[0].GetCharacterData();

        return null;
    }

    public void ResetAllEnergy()
    {
        for (int i = 0; i < PlayerCharactersList.Count; i++)
        {
            PlayerCharactersList[i].GetCharacterData().ResetEnergyCost();
        }
    }

    public void SetElementsIndicator(ElementsIndicator ElementsIndicator)
    {
        elementsIndicator = ElementsIndicator;
    }


    private void Start()
    {
        SubscribeToKeyInputs();
        SceneManager = SceneManager.GetInstance();
        SceneManager.OnSceneChanged += OnSceneChanged;
        onCharacterChange += CharacterChange;
        SwapCharacters(0);
    }

    private void SubscribeToKeyInputs()
    {
        if (playerController)
            playerController.OnNumsKeyInput += SwapCharactersControls;
    }

    private void OnSceneChanged()
    {
        PlayerCharactersList.Clear();
        inventoryManager.SpawnCharacters();
        SubscribeToKeyInputs();
        SwapCharacters(CurrentSelectionCharacterData);
    }

    private void OnDestroy()
    {
        if (playerController)
            playerController.OnNumsKeyInput -= SwapCharactersControls;
    }

    private void Update()
    {
        UpdateCharacterData();
    }

    private void UpdateCharacterData()
    {
        for (int i = 0; i < inventoryManager.GetCharactersOwnedList().Count; i++)
        {
            CharacterData characterData = inventoryManager.GetCharactersOwnedList()[i];
            characterData.Update();
        }
    }

    private void CharacterChange(CharacterData cd)
    {
        if (cd == null)
            return;

        AssetManager.GetInstance().SpawnParticlesEffect(GetPlayerCharacter(cd.GetPlayerCharacterSO()).transform.position, AssetManager.GetInstance().SwitchCharacterParticlesEffect);
    }

    private void SwapCharactersControls(float index)
    {
        if (playerController.isBurstState())
            return;

        if (GetPlayerController().GetPlayerState().GetPlayerMovementState() is PlayerDeadState || GetPlayerController().IsAiming())
            return;

        SwapCharacters((int)index - 1);
    }

    public void SwapCharacters(CharacterData characterData)
    {
        if (characterData == null)
            return;

        PlayerCharacters playerCharacters = GetPlayerCharacter(characterData.GetPlayerCharacterSO());
        ChangeCharacter(playerCharacters, characterData);
    }

    public void SwapCharacters(int index)
    {
        inventoryManager = InventoryManager.GetInstance();
        if (index >= inventoryManager.GetCharactersOwnedList().Count)
            return;

        PlayerCharacters playerCharacters = GetPlayerCharacter(GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);
        CharacterData characterData = inventoryManager.GetOwnedCharacterData(GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);

        ChangeCharacter(playerCharacters, characterData);
    }

    private void ChangeCharacter(PlayerCharacters playerCharacters, CharacterData characterData)
    {
        if (characterData != null && playerCharacters != null)
        {
            if (playerCharacters.IsDead())
                return;

            for (int i = 0; i < PlayerCharactersList.Count; i++)
            {
                PlayerCharactersList[i].gameObject.SetActive(false);
            }

            playerCharacters.SetCharacterData(characterData);
            playerCharacters.SetisAttacking(false);
            inventoryManager.SetCurrentEquipCharacter(characterData);
            SetCurrentCharacter(playerCharacters);
            playerCharacters.gameObject.SetActive(true);

            if (GetElementsIndicator() != null)
            {
                Destroy(GetElementsIndicator().gameObject);
            }
            CurrentSelectionCharacterData = characterData;
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
    public List<PlayerCharacters> GetPlayerCharactersList()
    {
        return PlayerCharactersList;
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
