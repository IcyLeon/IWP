using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsManager : MonoBehaviour
{
    [Header("Elemental Skill")]
    [SerializeField] CanvasGroup ElementalSkillBackgroundCanvasGroup;
    [SerializeField] Image ElementalSkillSprite;
    [SerializeField] TextMeshProUGUI ElementalSkillCooldownTxt;

    [Header("Elemental Burst")]
    [SerializeField] TextMeshProUGUI BurstSkillCooldownTxt;
    [SerializeField] Image ElementalBurstImage;
    [SerializeField] Sprite ElementalBurstBackgroundSprite_Default;
    [SerializeField] Image BurstFill;

    private CharacterData PlayerCharacterData;
    private ElementalReactionsManager elementalReactionsManager;

    // Start is called before the first frame update
    void Start()
    {
        elementalReactionsManager = ElementalReactionsManager.GetInstance();
        CharacterManager.GetInstance().onCharacterChange += OnCharacterSwitch;
        OnCharacterSwitch(InventoryManager.GetInstance().GetCurrentEquipCharacterData());
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCharacterData != null)
        {
            UpdateElementalSkill();
            UpdateElementalBurst();
        }

    }

    private void UpdateElementalBurst()
    {
        if (PlayerCharacterData.CanTriggerBurstSKillCost())
        {
            ElementalBurstImage.sprite = elementalReactionsManager.GetElementalColorSO().GetElementalInfo(PlayerCharacterData.GetPlayerCharacterSO().Elemental).ElementBurstBackground;
        }
        else
        {
            ElementalBurstImage.sprite = ElementalBurstBackgroundSprite_Default;
        }
        BurstFill.gameObject.SetActive(!PlayerCharacterData.CanTriggerBurstSKillCost());

        BurstSkillCooldownTxt.text = PlayerCharacterData.GetCurrentElementalBurstCooldown().ToString("0.0") + "s";
        BurstFill.fillAmount = PlayerCharacterData.GetCurrentEnergyBurstCost() / PlayerCharacterData.GetEnergyBurstCost();
        BurstSkillCooldownTxt.gameObject.SetActive(!PlayerCharacterData.CanTriggerBurstSKill());
    }

    private void UpdateElementalSkill()
    {
        ElementalSkillCooldownTxt.text = PlayerCharacterData.GetCurrentElementalSkillCooldown().ToString("0.0") + "s";
        if (PlayerCharacterData.CanTriggerESKill())
            ElementalSkillBackgroundCanvasGroup.alpha = 1f;
        else
            ElementalSkillBackgroundCanvasGroup.alpha = 0.5f;

        if (GetPlayerCharacterSO())
        {
            ElementalSkillSprite.sprite = GetPlayerCharacterSO().ElementalSkillSprite;
        }
        ElementalSkillCooldownTxt.gameObject.SetActive(!PlayerCharacterData.CanTriggerESKill());
    }

    void OnCharacterSwitch(CharacterData PlayerCharacterData)
    {
        this.PlayerCharacterData = PlayerCharacterData;
    }

    private PlayerCharacterSO GetPlayerCharacterSO()
    {
        if (PlayerCharacterData == null)
            return null;

        return PlayerCharacterData.GetItemSO() as PlayerCharacterSO;
    }
}
