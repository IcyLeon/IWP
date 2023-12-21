using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] Transform CameraLook;
    private Rigidbody rb;
    private List<PlayerCharacters> PlayerCharactersList = new();
    private PlayerCharacters CurrentCharacter;
    private PlayerController playerController;
    private ElementsIndicator elementsIndicator;

    private StaminaManager staminaManager;
    private InventoryManager inventoryManager;
    private CharacterManager characterManager;
    private SceneManager SceneManager;
    public delegate void OnCharacterChange(CharacterData characterData);
    public OnCharacterChange onCharacterChange;
    private Animator CharacterAnimatorReference;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        characterManager = CharacterManager.GetInstance();
        characterManager.SetPlayerManager(this);
    }

    private void Start()
    {
        inventoryManager = InventoryManager.GetInstance();
        staminaManager = StaminaManager.GetInstance();
        SceneManager = SceneManager.GetInstance();
        SceneManager.OnSceneChanged += OnSceneChanged;
        SubscribeToKeyInputs();
        StartCoroutine(SpawnInitDelay());
    }

    private IEnumerator SpawnInitDelay()
    {
        yield return null;
        SwapCharacters(inventoryManager.GetCurrentEquipCharacterData());
    }

    private void OnSceneChanged()
    {
        PlayerCharactersList.Clear();
        characterManager.SpawnCharacters(GetCharactersOwnedList());
        SubscribeToKeyInputs();
        SwapCharacters(inventoryManager.GetCurrentEquipCharacterData());
    }

    public void AddPlayerCharactersList(PlayerCharacters pc)
    {
        PlayerCharactersList.Add(pc);
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
            AssetManager.GetInstance().SpawnWorldText_Other(GetPlayerOffsetPosition().position, OthersState.HEAL, "+" + (int)actualamt);
            if (c == inventoryManager.GetCurrentEquipCharacterData())
            {
                ParticleSystem healEffect = Instantiate(AssetManager.GetInstance().HealEffect, transform).GetComponent<ParticleSystem>();
                Destroy(healEffect.gameObject, healEffect.main.duration);
            }
        }

    }

    private void CharacterChange(CharacterData cd)
    {
        if (cd == null)
            return;

        AssetManager.GetInstance().SpawnParticlesEffect(GetPlayerCharacter(cd.GetPlayerCharacterSO()).transform.position, AssetManager.GetInstance().SwitchCharacterParticlesEffect);
    }

    public ElementsIndicator GetElementsIndicator()
    {
        return elementsIndicator;
    }

    public List<CharacterData> GetCharactersOwnedList()
    {
        return inventoryManager.GetCharactersOwnedList();
    }
    public void AddAllCharactersPercentage(float percentage)
    {
        for (int i = 0; i < GetCharactersOwnedList().Count; i++)
        {
            PlayerCharactersList[i].SetHealth(PlayerCharactersList[i].GetHealth() + PlayerCharactersList[i].GetMaxHealth() * percentage);
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

    public void SetElementsIndicator(ElementsIndicator ElementsIndicator)
    {
        elementsIndicator = ElementsIndicator;
    }

    public void ResetAllEnergy()
    {
        for (int i = 0; i < PlayerCharactersList.Count; i++)
        {
            PlayerCharactersList[i].GetCharacterData().ResetEnergyCost();
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
            AssetManager.GetInstance().SpawnWorldText_Other(GetPlayerOffsetPosition().position, OthersState.HEAL, "+" + (int)actualamt);
    }

    private void SubscribeToKeyInputs()
    {
        if (playerController)
            playerController.OnNumsKeyInput += SwapCharactersControls;
    }

    private void OnDestroy()
    {
        if (playerController)
            playerController.OnNumsKeyInput -= SwapCharactersControls;

        SceneManager.OnSceneChanged -= OnSceneChanged;
        onCharacterChange -= CharacterChange;
    }


    public Transform GetPlayerOffsetPosition()
    {
        return CameraLook;
    }

    public bool CanPerformAction()
    {
        return (GetPlayerMovementState() is PlayerGroundState || GetPlayerMovementState() is PlayerAimState) &&
            !IsDashing() && !IsSkillCasting() &&
            !isBurstState();
    }

    public bool IsGrounded()
    {
        return GetPlayerMovementState() is not PlayerAirborneState;
    }

    public CharacterManager GetCharacterManager()
    {
        return characterManager;
    }

    public bool isBurstActive()
    {
        if (GetCurrentCharacter() == null)
            return false;

        return GetCurrentCharacter().GetBurstActive();
    }
    public bool CanAttack()
    {
        return (CanPerformAction() || GetPlayerMovementState() is PlayerAttackState) && !IsSkillCasting();
    }
    public bool isDeadState()
    {
        return GetPlayerMovementState() is PlayerDeadState;
    }

    public bool isBurstState()
    {
        return GetPlayerMovementState() is PlayerBurstState;
    }


    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public Animator GetAnimator()
    {
        if (CurrentCharacter == null)
            return null;

        return CurrentCharacter.GetAnimator();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOutofBound();
        UpdateCharacterData();
    }

    private void UpdateCharacterData()
    {
        if (inventoryManager == null)
            return;

        for (int i = 0; i < GetCharactersOwnedList().Count; i++)
        {
            CharacterData characterData = GetCharactersOwnedList()[i];
            characterData.Update();
        }
    }


    public bool IsAiming()
    {
        return GetPlayerMovementState() is PlayerAimState;
    }

    public bool IsMoving()
    {
        return GetPlayerMovementState() is PlayerMovingState;
    }

    public bool IsDashing()
    {
        return GetPlayerMovementState() is PlayerDashState;
    }

    public List<CharacterData> GetEquippedCharacterData()
    {
        return inventoryManager.GetEquipCharactersDataList();
    }

    public PlayerMovementState GetPlayerMovementState()
    {
        return GetPlayerController().GetPlayerState().GetPlayerMovementState();
    }

    private void UpdateOutofBound()
    {
        if (rb == null)
            return;

        if (rb.position.y <= -100f)
        {
            ResetAllEnergy();
            AddAllCharactersPercentage(-0.15f);
            rb.position = GetPlayerController().GetPlayerState().PlayerData.PreviousPosition;
        }
    }

    private void SwapCharactersControls(float index)
    {
        if (isBurstState())
            return;

        if (isDeadState() || IsAiming() || GetPlayerMovementState() is PlayerAirborneState)
            return;

        if (GetCurrentCharacter()?.GetPlayerCharacterState()?.GetPlayerControlState() is PlayerElementalSkillState || GetCurrentCharacter()?.GetPlayerCharacterState()?.GetPlayerControlState() is PlayerElementalBurstState)
            return;

        SwapCharacters((int)index - 1);
    }

    public void SwapCharacters(CharacterData characterData, bool showeffect = false)
    {
        if (characterData == null)
            return;

        PlayerCharacters playerCharacters = GetPlayerCharacter(characterData.GetPlayerCharacterSO());
        ChangeCharacter(playerCharacters, characterData, showeffect);
    }

    public void SwapCharacters(int index)
    {
        inventoryManager = InventoryManager.GetInstance();
        if (index >= GetCharactersOwnedList().Count)
            return;

        PlayerCharacters playerCharacters = GetPlayerCharacter(GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);
        CharacterData characterData = inventoryManager.GetOwnedCharacterData(GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);

        ChangeCharacter(playerCharacters, characterData, true);
    }

    private void ChangeCharacter(PlayerCharacters playerCharacters, CharacterData characterData, bool showeffects)
    {
        if (characterData != null && playerCharacters != null)
        {
            if (playerCharacters == GetCurrentCharacter())
                return;

            if (playerCharacters.IsDead())
                return;

            for (int i = 0; i < PlayerCharactersList.Count; i++)
            {
                PlayerCharactersList[i].gameObject.SetActive(false);
            }

            playerCharacters.SetCharacterData(characterData);
            playerCharacters.ResetAttack();
            inventoryManager.SetCurrentEquipCharacter(characterData);
            SetCurrentCharacter(playerCharacters);
            playerCharacters.gameObject.SetActive(true);

            if (GetElementsIndicator() != null)
                Destroy(GetElementsIndicator().gameObject);
       
            if (showeffects)
                CharacterChange(characterData);

            inventoryManager.SetCurrentEquipCharacter(characterData);
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

    public bool IsSkillCasting()
    {
        if (GetCurrentCharacter() == null)
            return false;

        if (GetCurrentCharacter().GetPlayerCharacterState() == null)
            return false;

        return (GetCurrentCharacter().GetPlayerCharacterState().GetPlayerControlState() is PlayerElementalSkillState || GetPlayerMovementState() is PlayerBurstState);
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

    public StaminaManager GetStaminaManager()
    {
        return staminaManager;
    }

    public Rigidbody GetCharacterRB()
    {
        return rb;
    }
}
