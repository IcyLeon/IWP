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
    [SerializeField] GameObject ElementalSkillContent;

    [Header("Elemental Burst")]
    [SerializeField] CanvasGroup ElementalBurstBackgroundCanvasGroup;
    [SerializeField] TextMeshProUGUI BurstSkillCooldownTxt;
    [SerializeField] Image ElementalBurstImage;
    [SerializeField] Image ElementalBurstSkillSprite;
    [SerializeField] Sprite ElementalBurstBackgroundSprite_Default;
    [SerializeField] Image BurstFill;
    [SerializeField] GameObject ElementalBurstContent;

    [Header("Gadget")]
    [SerializeField] CanvasGroup GadgetBackgroundCanvasGroup;
    [SerializeField] Image GadgetSkillSprite;
    [SerializeField] TextMeshProUGUI GadgetSkillCooldownTxt;
    [SerializeField] TextMeshProUGUI AmtTxt;
    [SerializeField] GameObject AmtContent;
    [SerializeField] GameObject GadgetContent;

    [Header("Aim")]
    [SerializeField] GameObject AimContent;

    private GadgetItem CurrentGadgetItem;
    private CharacterData CurrentPlayerCharacterData;
    private ElementalReactionsManager elementalReactionsManager;

    private void Awake()
    {
        PlayerManager.onCharacterChange += OnCharacterSwitch;
        PlayerManager.OnGadgetItemChange += OnGadgetItemChange;
    }
    // Start is called before the first frame update
    void Start()
    {
        elementalReactionsManager = ElementalReactionsManager.GetInstance();
    }

    private void OnGadgetItemChange(GadgetItem GadgetItem)
    {
        CurrentGadgetItem = GadgetItem;
    }

    private void OnDestroy()
    {
        PlayerManager.onCharacterChange -= OnCharacterSwitch;
        PlayerManager.OnGadgetItemChange -= OnGadgetItemChange;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateElementalSkill();
        UpdateElementalBurst();
        UpdateGadget();
    }

    private void UpdateGadget()
    {
        GadgetContent.SetActive(CurrentGadgetItem != null);
        if (CurrentGadgetItem == null)
        {
            return;
        }

        AmtContent.SetActive(CurrentGadgetItem is GadgetConsumableItem);

        switch (CurrentGadgetItem)
        {
            case GadgetConsumableItem GadgetConsumableItem:
                switch (GadgetConsumableItem)
                {
                    case FoodGadget foodGadget:
                        if (foodGadget.GetCurrentFood() != null)
                        {
                            GadgetSkillSprite.sprite = foodGadget.GetCurrentFood().GetItemSO().ItemSprite;
                        }
                        GadgetSkillSprite.enabled = foodGadget.GetCurrentFood() != null;
                        break;
                }
                AmtTxt.text = GadgetConsumableItem.GetAmount().ToString();
                break;
            default:
                GadgetSkillSprite.sprite = CurrentGadgetItem.GetItemSO().ItemSprite;
                break;
        }
    }

    private void UpdateElementalBurst()
    {
        ElementalBurstContent.SetActive(CurrentPlayerCharacterData != null);
        if (CurrentPlayerCharacterData == null)
        {
            return;
        }

        if (CurrentPlayerCharacterData.CanTriggerBurstSKillCost())
            ElementalBurstImage.sprite = elementalReactionsManager.GetElementalColorSO().GetElementalInfo(CurrentPlayerCharacterData.GetPlayerCharacterSO().Elemental).ElementBurstBackground;
        else
            ElementalBurstImage.sprite = ElementalBurstBackgroundSprite_Default;

        if (CurrentPlayerCharacterData.CanTriggerBurstSKill() && CurrentPlayerCharacterData.CanTriggerBurstSKillCost())
            ElementalBurstBackgroundCanvasGroup.alpha = 1f;
        else
            ElementalBurstBackgroundCanvasGroup.alpha = 0.5f;


        if (GetPlayerCharacterSO())
        {
            ElementalBurstSkillSprite.sprite = GetPlayerCharacterSO().ElementalBurstSprite;
        }

        BurstSkillCooldownTxt.text = CurrentPlayerCharacterData.GetCurrentElementalBurstCooldown().ToString("0.0") + "s";
        BurstFill.fillAmount = CurrentPlayerCharacterData.GetCurrentEnergyBurstCost() / CurrentPlayerCharacterData.GetEnergyBurstCost();
        BurstSkillCooldownTxt.gameObject.SetActive(!CurrentPlayerCharacterData.CanTriggerBurstSKill());
        BurstFill.gameObject.SetActive(!CurrentPlayerCharacterData.CanTriggerBurstSKillCost());

    }

    private void UpdateElementalSkill()
    {
        ElementalSkillContent.SetActive(CurrentPlayerCharacterData != null);
        if (CurrentPlayerCharacterData == null)
        {
            return;
        }

        ElementalSkillCooldownTxt.text = CurrentPlayerCharacterData.GetCurrentElementalSkillCooldown().ToString("0.0") + "s";
        if (CurrentPlayerCharacterData.CanTriggerESKill())
            ElementalSkillBackgroundCanvasGroup.alpha = 1f;
        else
            ElementalSkillBackgroundCanvasGroup.alpha = 0.5f;

        if (GetPlayerCharacterSO())
        {
            ElementalSkillSprite.sprite = GetPlayerCharacterSO().ElementalSkillSprite;
        }
        ElementalSkillCooldownTxt.gameObject.SetActive(!CurrentPlayerCharacterData.CanTriggerESKill());
    }

    void OnCharacterSwitch(CharacterData CurrentPlayerCharacterData, PlayerCharacters playerCharacters)
    {
        this.CurrentPlayerCharacterData = CurrentPlayerCharacterData;

        AimContent.SetActive(playerCharacters is BowCharacters);
    }

    private PlayerCharacterSO GetPlayerCharacterSO()
    {
        if (CurrentPlayerCharacterData == null)
            return null;

        return CurrentPlayerCharacterData.GetItemSO() as PlayerCharacterSO;
    }
}
