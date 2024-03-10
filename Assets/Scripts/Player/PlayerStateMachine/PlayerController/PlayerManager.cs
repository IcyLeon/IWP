using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerCanvasUI PlayerCanvasUI;
    [SerializeField] PlayerElementalSkillandBurstManager PlayerElementalSkillandBurstManager;

    [Header("Common Audio")]
    [SerializeField] AudioClip SwitchSoundEffectsClip;
    [Header("Effects")]
    [SerializeField] GameObject DashEffects;
    [SerializeField] GameObject SwitchCharacterEffect;

    private Rigidbody rb;
    private List<PlayerCharacters> PlayerCharactersList = new();
    private PlayerCharacters CurrentCharacter;

    private StaminaManager staminaManager;
    private InventoryManager inventoryManager;
    private CharacterManager characterManager;

    public delegate void OnCharacterChange(CharacterData characterData, PlayerCharacters playerCharacters);
    public static OnCharacterChange onCharacterChange;
    public static Action<ElementalReactionsTrigger, Elements, IDamage> onEntityHitSendInfo;
    public static Action<GadgetItem> OnGadgetItemChange = delegate { };
    private ItemCollectedUI ItemCollectedUI;
    private GadgetItem currentEquipGadgetItem;

    private PlayerState playerState;

    public static void CallEntityHitSendInfo(ElementalReactionsTrigger ER, Elements e, IDamage IDamage)
    {
        onEntityHitSendInfo?.Invoke(ER, e, IDamage);
    }

    public PlayerCanvasUI GetPlayerCanvasUI()
    {
        return PlayerCanvasUI;
    }

    private void ReviveAllCharacters()
    {
        for (int i = 0; i < GetCharactersOwnedList().Count; i++)
        {
            CharacterData pc = GetCharactersOwnedList()[i];
            HealCharacterBruteForce(pc, pc.GetActualMaxHealth(pc.GetLevel()));
        }
    }


    public PlayerElementalSkillandBurstManager GetPlayerElementalSkillandBurstManager()
    {
        return PlayerElementalSkillandBurstManager;
    }

    public void SpawnItemCollectedUI(ItemTemplate itemSO)
    {
        AssetManager AM = AssetManager.GetInstance();
        if (ItemCollectedUI != null)
        {
            Destroy(ItemCollectedUI.gameObject);
        }

        ItemCollectedUI = AM.SpawnItemCollectedUI(itemSO);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        characterManager = CharacterManager.GetInstance();
        characterManager.SetPlayerManager(this);
        PlayerController.OnGadgetUse += OnGadgetUse;
        SceneManager.OnSceneChanged += OnSceneChanged;
        FallenUI.OnReviveChange += ReviveAllCharacters;
        PlayerController.OnNumsKeyInput += SwapCharactersControls;
    }

    public void OnAnimationTransition()
    {
        playerState.OnAnimationTransition();
    }
    public PlayerState GetPlayerState()
    {
        return playerState;
    }
    private void Start()
    {
        playerState = new PlayerState(this, playerController);
        inventoryManager = InventoryManager.GetInstance();
        staminaManager = StaminaManager.GetInstance();
    }


    public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }
    public void SetCurrentGadgetItemEquipped(GadgetItem gadgetItem)
    {
        if (GetInventoryManager() == null)
            return;

        GetInventoryManager().SetCurrentGadgetItemEquipped(gadgetItem);
        currentEquipGadgetItem = GetInventoryManager().GetCurrentGadgetItem();
        OnGadgetItemChange?.Invoke(gadgetItem);
    }

    private void OnGadgetUse()
    {
        GadgetItem gadgetItem = GetInventoryManager().GetCurrentGadgetItem();
        if (gadgetItem != null)
        {
            gadgetItem.UseGadget();
        }
    }
    private IEnumerator SpawnInitDelay()
    {
        yield return null;
        characterManager.SpawnCharacters(GetCharactersOwnedList(), this);
        SwapCharacters(GetInventoryManager().GetCurrentEquipCharacterData());
        SetCurrentGadgetItemEquipped(GetInventoryManager().GetCurrentGadgetItem());
    }

    private void OnSceneChanged(SceneEnum s)
    {
        PlayerCharactersList.Clear();
        StartCoroutine(SpawnInitDelay());
    }

    public void AddPlayerCharactersList(PlayerCharacters pc)
    {
        PlayerCharactersList.Add(pc);
    }

    public void HealCharacter(CharacterData c, float amt, bool showtext = true)
    {
        if (c == null)
            return;

        if (c.IsDead())
            return;

        float actualamt = amt;
        if (actualamt < 1f)
            actualamt = 1f;

        c.SetHealth(c.GetHealth() + (int)actualamt);
        if (c.GetHealth() < c.GetActualMaxHealth(c.GetLevel()) && showtext)
        {
            if (c == GetInventoryManager().GetCurrentEquipCharacterData())
            {
                AssetManager.GetInstance().SpawnWorldText_Other(GetPlayerOffsetPosition().position, OthersState.HEAL, "+" + (int)actualamt);
                ParticleSystem healEffect = Instantiate(AssetManager.GetInstance().HealEffect, transform).GetComponent<ParticleSystem>();
                Destroy(healEffect.gameObject, healEffect.main.duration);
            }
        }

    }

    public void SpawnDashEffects(Vector3 dir)
    {
        Quaternion rotation = Quaternion.Euler(0f, 180 + Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, 0f);
        AssetManager.GetInstance().SpawnParticlesEffect(DashEffects, GetPlayerOffsetPosition().position, rotation);
    }

    private void CharacterChange(CharacterData cd)
    {
        if (cd == null)
            return;

        AssetManager.GetInstance().SpawnParticlesEffect(SwitchCharacterEffect, GetPlayerCharacter(cd.GetPlayerCharacterSO()).transform.position);
    }

    public List<CharacterData> GetCharactersOwnedList()
    {
        return GetInventoryManager().GetCharactersOwnedList();
    }
    public void AddHealthAllCharactersPercentage(float percentage)
    {
        for (int i = 0; i < GetCharactersOwnedList().Count; i++)
        {
            PlayerCharactersList[i].SetHealth(PlayerCharactersList[i].GetHealth() + PlayerCharactersList[i].GetMaxHealth() * percentage);
        }
    }

    public CharacterData GetDeadCharacter()
    {
        List<PlayerCharacters> PlayerCharacerListCopy = new(PlayerCharactersList);
        for (int i = PlayerCharacerListCopy.Count - 1; i >= 0; i--)
        {
            if (!PlayerCharacerListCopy[i].IsDead())
            {
                PlayerCharacerListCopy.RemoveAt(i);
            }
        }

        if (PlayerCharacerListCopy.Count > 0)
            return PlayerCharacerListCopy[0].GetCharacterData();

        return null;
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

    public void HealCharacterBruteForce(CharacterData c, float amt, bool showtext = true)
    {
        if (c == null)
            return;

        float actualamt = amt;
        if (actualamt < 1f)
            actualamt = 1f;

        c.SetHealth(c.GetHealth() + (int)actualamt);
        if (c.GetHealth() < c.GetActualMaxHealth(c.GetLevel()) && showtext)
        {
            if (c == GetInventoryManager().GetCurrentEquipCharacterData())
            {
                AssetManager.GetInstance().SpawnWorldText_Other(GetPlayerOffsetPosition().position, OthersState.HEAL, "+" + (int)actualamt);
                ParticleSystem healEffect = Instantiate(AssetManager.GetInstance().HealEffect, transform).GetComponent<ParticleSystem>();
                Destroy(healEffect.gameObject, healEffect.main.duration);
            }
        }
    }

    private void OnDestroy()
    {
        PlayerController.OnNumsKeyInput -= SwapCharactersControls;
        PlayerController.OnGadgetUse -= OnGadgetUse;
        FallenUI.OnReviveChange -= ReviveAllCharacters;
        SceneManager.OnSceneChanged -= OnSceneChanged;
    }


    public Transform GetPlayerOffsetPosition()
    {
        return playerController.GetCameraManager().GetOffsetCameraLook();
    }

    public bool CanPerformAction()
    {
        return (GetPlayerMovementState() is PlayerGroundState || GetPlayerMovementState() is PlayerAttackState) &&
            !IsSkillCasting() &&
            !IsBurstState();
    }

    public bool IsGrounded()
    {
        return GetPlayerMovementState() is not PlayerAirborneState;
    }

    public CharacterManager GetCharacterManager()
    {
        return characterManager;
    }

    public bool IsBurstActive()
    {
        if (GetCurrentCharacter() == null)
            return false;

        return GetCurrentCharacter().GetBurstActive();
    }

    public bool IsDeadState()
    {
        return GetPlayerMovementState() is PlayerDeadState;
    }

    public bool IsBurstState()
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
        UpdatePlayerState();
        UpdateGadget();
        UpdateCharacterData();
    }

    void UpdatePlayerState()
    {
        if (GetPlayerState() == null)
            return;

        if (Time.timeScale == 0)
            return;

        GetPlayerState().Update();
    }

    void FixedUpdate()
    {
        if (GetPlayerState() != null)
            GetPlayerState().FixedUpdate();    
    }

    public float GetAnimationSpeed()
    {
        return playerState.GetPlayerMovementState().GetAnimationSpeed();
    }

    private void UpdateGadget()
    {
        if (currentEquipGadgetItem == null || Time.timeScale == 0)
            return;

        currentEquipGadgetItem.Update();
    }
    private void UpdateCharacterData()
    {
        if (GetInventoryManager() == null)
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
        return GetInventoryManager().GetEquipCharactersDataList();
    }

    public PlayerMovementState GetPlayerMovementState()
    {
        return GetPlayerState().GetPlayerMovementState();
    }

    private void SwapCharactersControls(float index)
    {
        if (IsSkillCasting() || IsDeadState() || IsAiming() || GetPlayerMovementState() is PlayerAirborneState)
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
        CharacterData characterData = GetInventoryManager().GetOwnedCharacterData(GetCharactersOwnedList()[index].GetItemSO() as PlayerCharacterSO);
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

            foreach(PlayerCharacters pc in PlayerCharactersList)
                pc.gameObject.SetActive(false);

            playerCharacters.SetCharacterData(characterData);
            playerCharacters.SetisAttacking(false);
            playerCharacters.ResetElementsIndicator();
            GetInventoryManager().SetCurrentEquipCharacter(characterData);
            SetCurrentCharacter(playerCharacters);
            playerCharacters.gameObject.SetActive(true);

            if (showeffects)
                CharacterChange(characterData);

            GetInventoryManager().SetCurrentEquipCharacter(characterData);
            SoundEffectsManager.GetInstance().PlaySFXSound(SwitchSoundEffectsClip);
            onCharacterChange?.Invoke(CurrentCharacter.GetCharacterData(), CurrentCharacter);
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

        return (GetCurrentCharacter().GetPlayerCharacterState().GetCurrentState() is PlayerElementalSkillState || IsBurstState());
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
