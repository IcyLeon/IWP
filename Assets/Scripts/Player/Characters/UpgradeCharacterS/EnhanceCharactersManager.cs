using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class EnhanceCharactersManager : MonoBehaviour
{
    [SerializeField] UpgradeCharacterSO UpgradeCharacterSO;
    [SerializeField] GameObject MaxLevelContent;
    [SerializeField] GameObject EnhancementContent;
    [SerializeField] GameObject ButtonMask;
    [SerializeField] Button UpgradeBtn;
    [SerializeField] Transform SlotsParent;
    [SerializeField] TextMeshProUGUI LevelDisplay, ExpAmountDisplay, IncreaseAmountExpDisplay, IncreaseLevelDisplay;

    [Header("Amount Use Manager")]
    [SerializeField] TextMeshProUGUI AmountAddTxt;
    [SerializeField] Button DecreaseBtn;
    [SerializeField] Button AddBtn;

    private CharactersShowcaseManager CharactersShowcaseManager;
    private ExpItemSO SelectedExpItemSO;
    private CharacterData characterData;
    private float PreviewUpgradeEXP;
    [SerializeField] ExpItemSO[] ExpItemSOList;

    private Dictionary<ExpItemSO, ItemButton> itembutton_Dictionary = new();
    private Dictionary<ExpItemSO, int> GiveExp_Dictionary = new();
    [SerializeField] Slider upgradeProgressSlider, previewProgressSlider;
    private bool UpgradinginProgress = false;
    private InventoryManager IM;

    private int GetActualLevel()
    {
        return GetCharacterData().GetLevel() - 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        UpgradeBtn.onClick.AddListener(UpgradeItem);
        SetupMaterialItems();
        AddBtn.onClick.AddListener(
            () => AddandRemove(1)
        );
        DecreaseBtn.onClick.AddListener(
            () => AddandRemove(-1)
        );
    }

    private void AddandRemove(int value)
    {
        if (SelectedExpItemSO == null)
            return;

        GiveExp_Dictionary[SelectedExpItemSO] += value;
        GiveExp_Dictionary[SelectedExpItemSO] = Mathf.Clamp(GiveExp_Dictionary[SelectedExpItemSO], 0, IM.CountNumberOfItems(SelectedExpItemSO));

        UpdateAmountAddTxt();
    }

    private void UpdateAmountAddTxt()
    {
        if (SelectedExpItemSO == null)
            return;

        AmountAddTxt.text = GiveExp_Dictionary[SelectedExpItemSO].ToString();
    }

    private void SetupMaterialItems()
    {
        IM = InventoryManager.GetInstance();
        IM.OnInventoryChanged += OnInventoryChanged;

        for (int i = 0; i < ExpItemSOList.Length; i++)
        {
            ItemButton itemButton = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, SlotsParent).GetComponent<ItemButton>();
            itemButton.SetItemsSO(ExpItemSOList[i]);
            itemButton.onButtonClick += GetItemSelected;
            itemButton.SetDisplayText(IM.CountNumberOfItems(ExpItemSOList[i]).ToString());

            GiveExp_Dictionary.Add(ExpItemSOList[i], 0);
            itembutton_Dictionary.Add(ExpItemSOList[i], itemButton);
        }

        if (ExpItemSOList.Length > 0)
        {
            GetItemSelected(itembutton_Dictionary[ExpItemSOList[0]]);
        }
    }

    private void OnInventoryChanged(Item item, ItemTemplate itemSO)
    {
        ExpItemSO expItemSO = itemSO as ExpItemSO;
        if (expItemSO == null)
            return;

        if (itembutton_Dictionary.TryGetValue(expItemSO, out ItemButton itemButton)) {
            ConsumableItem ConsumableItem = (ConsumableItem)item;
            itemButton.SetDisplayText(ConsumableItem.GetAmount().ToString());
        }
    }

    private void OnDestroy()
    {
        IM.OnInventoryChanged -= OnInventoryChanged;

        for(int i = 0; i < itembutton_Dictionary.Count; i++)
        {
            KeyValuePair<ExpItemSO, ItemButton> itembutton_Dictionary_KeyPair = itembutton_Dictionary.ElementAt(i);
            if (itembutton_Dictionary.TryGetValue(itembutton_Dictionary_KeyPair.Key, out ItemButton itemButton)) {
                if (itemButton != null)
                    Destroy(itemButton.gameObject);
                itembutton_Dictionary.Remove(itembutton_Dictionary_KeyPair.Key);
            }
        }
    }

    private void UpdateOutlineSelection(ItemButton selecteditemButton)
    {
        foreach (ItemButton itemButton in itembutton_Dictionary.Values)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }

        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, selecteditemButton);
    }


    private void GetItemSelected(ItemButton itemButton)
    {
        SelectedExpItemSO = itemButton.GetItemsSO() as ExpItemSO;
        UpdateOutlineSelection(itemButton);
        UpdateAmountAddTxt();
    }


    // Update is called once per frame
    void Update()
    {
        if (!UpgradinginProgress)
        {
            UpdateContent();
            UpdatePreviewEXP();
        }
    }

    public void Init(CharactersShowcaseManager CharactersShowcaseManager, CharacterData c)
    {
        this.CharactersShowcaseManager = CharactersShowcaseManager;
        characterData = c;
    }

    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    private void UpdatePreviewEXP()
    {
        if (GetCharacterData() == null)
            return;

        PreviewUpgradeEXP = GetCharacterData().GetExpAmount() + GetTotalEXP();
        previewProgressSlider.minValue = upgradeProgressSlider.minValue;
        previewProgressSlider.maxValue = upgradeProgressSlider.maxValue;
        previewProgressSlider.value = PreviewUpgradeEXP;
    }

    private float GetTotalEXP()
    {
        float total = 0;

        foreach(var item in GiveExp_Dictionary)
        {
            total += item.Key.ExpAmount * item.Value;
        }
        return total;
    }

    public void SetExpDisplay()
    {
        if (GetCharacterData() == null)
            return;

        if (GetCharacterData().GetLevel() < UpgradeCharacterSO.CharacterUpgradeCost.Length)
            SetProgressUpgrades(0, UpgradeCharacterSO.CharacterUpgradeCost[GetActualLevel()]);
        else
            SetProgressUpgrades(0, 0);

        upgradeProgressSlider.value = GetCharacterData().GetExpAmount();
    }

    public void OpenEnhanceCharacterCanvas()
    {
        SetExpDisplay();
        gameObject.SetActive(true);
    }

    private void SetProgressUpgrades(float min, float max)
    {
        upgradeProgressSlider.minValue = min;
        upgradeProgressSlider.maxValue = max;
    }

    private int GetLevelIncrease()
    {
        int increaseLevel = 0;
        if (GetCharacterData() == null)
            return increaseLevel;

        float current = PreviewUpgradeEXP;

        for (int i = 0; i < UpgradeCharacterSO.CharacterUpgradeCost.Length; i++)
        {
            if (i >= GetCharacterData().GetLevel())
            {
                if (current < UpgradeCharacterSO.CharacterUpgradeCost[i])
                    break;

                current -= UpgradeCharacterSO.CharacterUpgradeCost[i];
                increaseLevel++;
            }
        }
        return increaseLevel;
    }

    private void UpdateContent()
    {
        if (GetCharacterData() != null)
        {
            LevelDisplay.text = "Lv. " + GetCharacterData().GetLevel();
            ExpAmountDisplay.text = Mathf.RoundToInt(upgradeProgressSlider.value) + "/" + upgradeProgressSlider.maxValue.ToString();

            IncreaseAmountExpDisplay.text = "+" + ((int)GetTotalEXP());
            IncreaseAmountExpDisplay.gameObject.SetActive((int)GetTotalEXP() != 0);

            IncreaseLevelDisplay.text = "+" + GetLevelIncrease();
            IncreaseLevelDisplay.gameObject.SetActive(GetLevelIncrease() != 0);

            MaxLevelContent.SetActive(GetCharacterData().GetLevel() == UpgradeCharacterSO.CharacterUpgradeCost.Length);
            EnhancementContent.SetActive(GetCharacterData().GetLevel() < UpgradeCharacterSO.CharacterUpgradeCost.Length);
        }
    }

    private IEnumerator UpgradeProgress()
    {
        float smoothTime = 4.5f;
        float UpgradeElasped = 0;
        ButtonMask.SetActive(true);

        while (GetCharacterData() != null)
        {
            UpgradinginProgress = true;
            if (GetCharacterData().GetLevel() < UpgradeCharacterSO.CharacterUpgradeCost.Length)
            {
                if (Mathf.Approximately(Mathf.RoundToInt(upgradeProgressSlider.value), (int)upgradeProgressSlider.maxValue))
                {
                    GetCharacterData().SetExpAmount(GetCharacterData().GetExpAmount() - upgradeProgressSlider.maxValue);
                    if ((GetActualLevel() + 1) < UpgradeCharacterSO.CharacterUpgradeCost.Length)
                        SetProgressUpgrades(0, UpgradeCharacterSO.CharacterUpgradeCost[GetActualLevel() + 1]);

                    UpdatePreviewEXP();
                    upgradeProgressSlider.value = upgradeProgressSlider.minValue;
                    GetCharacterData().Upgrade();
                }

                if (!Mathf.Approximately(Mathf.RoundToInt(upgradeProgressSlider.value), (int)GetCharacterData().GetExpAmount()))
                    upgradeProgressSlider.value = Mathf.Lerp(upgradeProgressSlider.value, GetCharacterData().GetExpAmount(), UpgradeElasped / smoothTime);
                else
                    break;
            }
            else
            {
                SetProgressUpgrades(0, 0); // max level
                UpdateContent();
                break;
            }

            UpgradeElasped += Time.unscaledDeltaTime;
            yield return null;
        }
        upgradeProgressSlider.value = GetCharacterData().GetExpAmount();
        ButtonMask.SetActive(false);
        UpgradinginProgress = false;
    }

    private void EnhanceUpgrade()
    {
        if (GetCharacterData() != null)
        {
            GetCharacterData().SetExpAmount(PreviewUpgradeEXP);
            StartCoroutine(UpgradeProgress());
        }

        for (int i = 0; i < GiveExp_Dictionary.Count; i++)
        {
            KeyValuePair<ExpItemSO, int> itembutton_Dictionary_KeyPair = GiveExp_Dictionary.ElementAt(i);
            if (GiveExp_Dictionary.TryGetValue(itembutton_Dictionary_KeyPair.Key, out int value)) {

                Item item = IM.GetItemREF(itembutton_Dictionary_KeyPair.Key);
                ConsumableItem cItem = item as ConsumableItem;
                if (cItem != null)
                {
                    cItem.Use(value);
                }

                GiveExp_Dictionary[itembutton_Dictionary_KeyPair.Key] = 0;
            }
        }

        UpdateAmountAddTxt();
    }

    private void UpgradeItem()
    {
        if ((int)GetTotalEXP() == 0)
            return;

        EnhanceUpgrade();
    }
}
