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
    [SerializeField] Image ElementalBurstSprite;
    [SerializeField] Image BurstFill;

    private CharacterData PlayerCharacterData;

    // Start is called before the first frame update
    void Start()
    {
        CharacterManager.GetInstance().onCharacterChange += OnCharacterSwitch;
        OnCharacterSwitch(InventoryManager.GetInstance().GetCurrentEquipCharacterData());
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCharacterData != null)
        {
            UpdateElementalSkill();
        }

    }

    private void UpdateElementalBurst()
    {
    }

    private void UpdateElementalSkill()
    {
        ElementalSkillCooldownTxt.text = PlayerCharacterData.GetCurrentElementalSkillCooldown().ToString("0.0") + "s";
        if (PlayerCharacterData.CanTriggerSKill())
            ElementalSkillBackgroundCanvasGroup.alpha = 1f;
        else
            ElementalSkillBackgroundCanvasGroup.alpha = 0.5f;

        if (GetPlayerCharacterSO())
        {
            ElementalSkillSprite.sprite = GetPlayerCharacterSO().ElementalSkillSprite;
        }
        ElementalSkillCooldownTxt.gameObject.SetActive(!PlayerCharacterData.CanTriggerSKill());
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
