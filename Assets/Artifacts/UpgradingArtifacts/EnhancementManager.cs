using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class EnhancementManager : MonoBehaviour
{
    [SerializeField] GameObject MaxLevelContent;
    [SerializeField] GameObject EnhancementContent;
    [SerializeField] GameObject ButtonMask;
    [Header("Enhancement Infomation")]
    [SerializeField] int EnhancementItemToSell;
    [SerializeField] SelectedArtifactsBubble SelectedArtifactsBubble;
    [SerializeField] TextMeshProUGUI LevelDisplay, ExpAmountDisplay, IncreaseAmountExpDisplay, IncreaseLevelDisplay;
    [SerializeField] Button AutoAddBtn;
    [SerializeField] Button UpgradeBtn;
    [SerializeField] Button ClearAllBtn;
    [SerializeField] Slider upgradeProgressSlider, previewProgressSlider;
    [SerializeField] TMP_Dropdown AutoAddSelection;
    [SerializeField] ItemsList itemList;
    [SerializeField] Transform SlotsParent;
    List<GameObject> EnhancementItemList = new List<GameObject>();
    private int NoofItemsAdded;
    private int RaritySelection;
    private int[] CostList;
    private float PreviewUpgradeEXP;
    private bool UpgradinginProgress = false;
    private InventoryManager InventoryManager;
    private UpgradeCanvas upgradeCanvas;

    [Header("Artifacts Stats")]
    [SerializeField] GameObject[] ArtifactsStatsContainer;
    [SerializeField] DisplayUpgradePossibleNotification RandomUpgradeStatsTxt;
    [SerializeField] DisplayUpgradePossibleNotification RandomAddStatsTxt;

    // Start is called before the first frame update
    void Start()
    {
        Init();

        InventoryManager = InventoryManager.GetInstance();
        InventoryManager.OnInventoryItemRemove += OnInventoryItemRemove;
        LoadEmptySlots();
        AutoAddBtn.onClick.AddListener(AutoAdd);
        UpgradeBtn.onClick.AddListener(UpgradeItem);
        ClearAllBtn.onClick.AddListener(ClearAll);
        AutoAddSelection.onValueChanged.AddListener(delegate
        {
            RaritySelection = AutoAddSelection.value;
        }
        );
    }

    public void Init()
    {
        if (upgradeCanvas == null)
            upgradeCanvas = MainUI.GetInstance().GetUpgradeCanvas();
    }
    public void SetExpDisplay()
    {
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;

        CostList = itemList.GetCostListStatus(GetItemREF().GetRarity());
        if (UpgradableItemREF.GetLevel() < CostList.Length)
            MainUI.SetProgressUpgrades(upgradeProgressSlider, 0, CostList[UpgradableItemREF.GetLevel()]);
        else
            MainUI.SetProgressUpgrades(upgradeProgressSlider, 0, 0);

        upgradeProgressSlider.value = UpgradableItemREF.GetExpAmount();
    }

    private void Update()
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton())
            {
                if (slot.GetItemButton().GetItemREF() is UpgradableItems upgradableItem && upgradableItem.GetLockStatus())
                {
                    slot.SetItemButton(null);
                }
            }
        }

        HideItem();
        ClearAllBtn.gameObject.SetActive(GetNoofSlotsTaken() != 0);

        if (!UpgradinginProgress)
        {
            UpdateContent();
            UpdatePreviewEXP();
        }
    }

    private void HideItem()
    {
        if (upgradeCanvas == null)
            return;

        upgradeCanvas.SlotPopup.HideItem(GetItemREF());

        if (upgradeCanvas.SlotPopup.GetItemButton_Dictionary() != null)
        {
            foreach(var go in upgradeCanvas.SlotPopup.GetItemButton_Dictionary().Keys)
            {
                ItemButton itemButton = upgradeCanvas.SlotPopup.GetItemButton_Dictionary()[go];
                if (itemButton.GetItemREF() is Artifacts)
                {
                    Artifacts artifacts = itemButton.GetItemREF() as Artifacts;
                    if (artifacts.GetCharacterEquipped() != null)
                    {
                        upgradeCanvas.SlotPopup.HideItem(artifacts);
                    }
                }
                else
                {
                    upgradeCanvas.SlotPopup.HideItem(itemButton.GetItemREF());
                }
            }
        }
    }

    public Item GetItemREF()
    {
        if (upgradeCanvas == null)
            return null;

        return upgradeCanvas.GetItemREF();
    }

    private ItemButton CheckIfItemalreadyExist(Item item)
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null) {
                if (slot.GetItemButton().GetItemREF() == item)
                    return slot.GetItemButton();
            }
        }
        return null;
    }


    private Slot GetSlotAt(ItemButton itembutton)
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
            {
                if (slot.GetItemButton().GetItemREF() == itembutton.GetItemREF())
                {
                    return slot;
                }
            }
        }
        return null;
    }

    private Slot FindAvailableSlot()
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();

            if (slot.GetItemButton() == null)
            {
                return slot;
            }
            else
            {
                if (slot.GetItemButton().GetItemREF() == null)
                    return slot;
            }
        }
        return null;
    }

    private int GetLevelIncrease()
    {
        int increaseLevel = 0;
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
        if (UpgradableItemREF == null)
            return increaseLevel;

        float current = PreviewUpgradeEXP;

        for (int i = 0; i < CostList.Length; i++)
        {
            if (i >= UpgradableItemREF.GetLevel())
            {
                if (current < CostList[i])
                    break;

                current -= CostList[i];
                increaseLevel++;
            }
        }
        return increaseLevel;
    }

    private void UpdatePreviewEXP()
    {
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
        if (UpgradableItemREF == null)
            return;

        PreviewUpgradeEXP = UpgradableItemREF.GetExpAmount() + GetTotalEXP();
        previewProgressSlider.minValue = upgradeProgressSlider.minValue;
        previewProgressSlider.maxValue = upgradeProgressSlider.maxValue;
        previewProgressSlider.value = PreviewUpgradeEXP;
    }

    private void UpdateContent()
    {
        if (GetItemREF() != null)
        {
            UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
            SelectedArtifactsBubble.UpdateSelectedItem((Artifacts)GetItemREF(), (ArtifactsSO)UpgradableItemREF.GetItemSO());
            LevelDisplay.text = "+" + UpgradableItemREF.GetLevel();
            ExpAmountDisplay.text = Mathf.RoundToInt(upgradeProgressSlider.value) + "/" + upgradeProgressSlider.maxValue.ToString();

            IncreaseAmountExpDisplay.text = "+" + ((int)GetTotalEXP());
            IncreaseAmountExpDisplay.gameObject.SetActive(GetNoofSlotsTaken() != 0);

            IncreaseLevelDisplay.text = "+" + GetLevelIncrease();
            IncreaseLevelDisplay.gameObject.SetActive(GetLevelIncrease() != 0);

            MaxLevelContent.SetActive(UpgradableItemREF.GetLevel() == CostList.Length);
            EnhancementContent.SetActive(UpgradableItemREF.GetLevel() < CostList.Length);
            UpdateArtifactStats();

            Artifacts selectedartifacts = UpgradableItemREF as Artifacts;
            for (int i = 0; i < ArtifactsStatsContainer.Length; i++)
            {
                DisplayArtifactStats stats = ArtifactsStatsContainer[i].GetComponent<DisplayArtifactStats>();
                if (stats != null)
                {
                    if (i <= selectedartifacts.GetTotalSubstatsDisplay())
                    {
                        stats.DisplayArtifactsStat(selectedartifacts.GetArtifactStatsName(i), selectedartifacts.GetStats(i), selectedartifacts);
                        if (i == 0)
                            stats.DisplayIncreaseValueMainStats(GetLevelIncrease(), selectedartifacts.GetStats(i));
                        stats.gameObject.SetActive(true);
                    }
                    else
                        stats.gameObject.SetActive(false);
                }
            }
        }
    }

    private int GetTotalNumberofSubStatsUpgrade(Artifacts artifacts)
    {
        int totalIncrease = 0;

        for (int i = artifacts.GetLevel(); i <= artifacts.GetLevel() + GetLevelIncrease(); i++)
        {
            if (i % ArtifactsManager.GetStatsActionConst() == 0 && i != artifacts.GetLevel())
            {
                totalIncrease++;
            }
        }
        return totalIncrease;
    }
    private void UpdateArtifactStats()
    {
        Artifacts artifacts = GetItemREF() as Artifacts;
        if (artifacts != null)
        {
            RandomUpgradeStatsTxt.UpdateNotification("Randomly upgrades " + ExcludeSubStatsMissing(artifacts) + " bonus stat(s)");
            RandomUpgradeStatsTxt.gameObject.SetActive(ExcludeSubStatsMissing(artifacts) > 0);

            RandomAddStatsTxt.UpdateNotification("Adds " + NoofMissingStatsReiterate(artifacts) + " new bonus stat(s)");
            RandomAddStatsTxt.gameObject.SetActive(NoofMissingStatsReiterate(artifacts) > 0 && artifacts.GetTotalSubstatsDisplay() != artifacts.GetLowestNumberofStats(artifacts.GetRarity()).MaxDropValue);

        }
    }

    private int NoofMissingStatsReiterate(Artifacts artifacts)
    {
        int NoOfStatsAction = GetTotalNumberofSubStatsUpgrade(artifacts);
        int diff = artifacts.GetLowestNumberofStats(artifacts.GetRarity()).MaxDropValue - artifacts.GetTotalSubstatsDisplay();
        int total = 0;

        for (int i = 0; i <= NoOfStatsAction; i++)
        {
            diff -= i;

            if (diff > 0 && NoOfStatsAction != 0)
            {
                total++;
            }
        }

        return total;
    }

    private int ExcludeSubStatsMissing(Artifacts artifacts)
    {
        int NoOfStatsAction = GetTotalNumberofSubStatsUpgrade(artifacts);
        int diff = artifacts.GetLowestNumberofStats(artifacts.GetRarity()).MaxDropValue - artifacts.GetTotalSubstatsDisplay();

        return NoOfStatsAction - diff;
    }


    private int GetTotalSlots()
    {
        return EnhancementItemList.Count;
    }

    private int GetNoofSlotsTaken()
    {
        int total = 0;
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
                if (slot.GetItemButton().GetItemREF() != null)
                    total++;
        }
        return total;
    }

    private List<GameObject> GetEnhancementItemList()
    {
        return EnhancementItemList;
    }

    private void OnDestroy()
    {
        if (upgradeCanvas == null)
            return;

        for (int i = EnhancementItemList.Count - 1; i >= 0; i--)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            upgradeCanvas.SlotPopup.UnSubscribeSlot(slot);
            slot.SlotItemButtonChanged -= OnSlotItemButtonChanged;
            Destroy(EnhancementItemList[i].gameObject);
        }
        EnhancementItemList.Clear();

        upgradeCanvas.SlotPopup.onSlotSend -= ManualAddItems;
        upgradeCanvas.SlotPopup.onSlotItemRemove -= ManualRemoveItems;
    }

    private void LoadEmptySlots()
    {
        if (upgradeCanvas == null)
            return;

        for (int i = 0; i < EnhancementItemToSell; i++)
        {
            GameObject enhancementitemslot = Instantiate(AssetManager.GetInstance().SlotPrefab, SlotsParent);
            Slot slot = enhancementitemslot.GetComponent<Slot>();
            upgradeCanvas.SlotPopup.SubscribeSlot(slot);
            slot.SlotItemButtonChanged += OnSlotItemButtonChanged;
            EnhancementItemList.Add(enhancementitemslot);
        }
        upgradeCanvas.SlotPopup.onSlotSend += ManualAddItems;
        upgradeCanvas.SlotPopup.onSlotItemRemove += ManualRemoveItems;
    }

    private void OnSlotItemButtonChanged(SendSlotInfo sendslotInfo, Item item)
    {
        if (sendslotInfo.slot)
        {
            if (!sendslotInfo.itemButtonREF)
                return;

            if (upgradeCanvas == null)
                return;

            if (upgradeCanvas.SlotPopup.GetItemButton_Dictionary().TryGetValue(sendslotInfo.itemButtonREF.GetItemREF(), out ItemButton SlotPopupitemButton))
            {
                SlotPopupitemButton.ToggleRemoveItemImage(item != null);
            }
        }
    }

    private void OnInventoryItemRemove(Item item, ItemTemplate itemSO)
    {
        ClearAll();
    }

    private void ManualRemoveItems(object sender, SendSlotInfo e)
    {
        ItemButton ExistitemButton = CheckIfItemalreadyExist(e.itemButtonREF.GetItemREF());

        if (ExistitemButton != null)
            GetSlotAt(ExistitemButton).SetItemButton(null);
    }

    public void ClearAll()
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
                GetSlotAt(slot.GetItemButton()).SetItemButton(null);
        }
    }

    private void ManualAddItems(object sender, SendSlotInfo e)
    {
        ItemButton ExistitemButton = CheckIfItemalreadyExist(e.itemButtonREF.GetItemREF());

        if (ExistitemButton == null)
        {
            UpgradableItems upgradableItems = e.itemButtonREF.GetItemREF() as UpgradableItems;
            if (upgradableItems != null)
            {
                if (upgradableItems.GetLockStatus())
                    return;

                if (upgradableItems is Artifacts)
                {
                    Artifacts artifacts = upgradableItems as Artifacts;
                    if (artifacts.GetCharacterEquipped() != null)
                        return;
                }
            }

            if (FindAvailableSlot() != null)
            {
                if (!isMaxError())
                    FindAvailableSlot().SetItemButton(e.itemButtonREF.GetItemREF());
                else
                    DisplayMaxError();
            }
            else
            {
                DisplayFullError();
            }
        }
    }


    private IEnumerator UpgradeProgress()
    {
        float smoothTime = 4.5f;
        float UpgradeElasped = 0;
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
        ButtonMask.SetActive(true);

        while (GetItemREF() != null)
        {
            UpgradinginProgress = true;
            if (UpgradableItemREF.GetLevel() < CostList.Length)
            {
                if (Mathf.Approximately(Mathf.RoundToInt(upgradeProgressSlider.value), (int)upgradeProgressSlider.maxValue))
                {
                    UpgradableItemREF.SetExpAmount(UpgradableItemREF.GetExpAmount() - upgradeProgressSlider.maxValue);
                    if ((UpgradableItemREF.GetLevel() + 1) < CostList.Length)
                        MainUI.SetProgressUpgrades(upgradeProgressSlider, 0, CostList[UpgradableItemREF.GetLevel() + 1]);

                    UpdatePreviewEXP();
                    upgradeProgressSlider.value = upgradeProgressSlider.minValue;
                    UpgradableItemREF.Upgrade();
                }

                if (!Mathf.Approximately(Mathf.RoundToInt(upgradeProgressSlider.value), (int)UpgradableItemREF.GetExpAmount()))
                    upgradeProgressSlider.value = Mathf.Lerp(upgradeProgressSlider.value, UpgradableItemREF.GetExpAmount(), UpgradeElasped / smoothTime);
                else
                    break;
            }
            else
            {
                MainUI.SetProgressUpgrades(upgradeProgressSlider, 0, 0); // max level
                UpdateContent();
                break;
            }

            UpgradeElasped += Time.unscaledDeltaTime;
            yield return null;
        }
        upgradeProgressSlider.value = UpgradableItemREF.GetExpAmount();
        ButtonMask.SetActive(false);
        UpgradinginProgress = false;
    }

    private float GetTotalEXP()
    {
        float total = 0;

        for (int i = 0; i < GetEnhancementItemList().Count; i++)
        {
            Slot slot = GetEnhancementItemList()[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
            {
                switch (slot.GetItemButton().GetItemREF()?.GetRarity())
                {
                    case Rarity.ThreeStar: // hardcode for now
                        total += 500f;
                        break;
                    case Rarity.FourStar:
                        total += 1000f;
                        break;
                    case Rarity.FiveStar:
                        total += 5000f;
                        break;
                }
            }
        }

        return total;
    }
    private void EnhanceUpgrade()
    {
        if (GetItemREF() != null)
        {
            UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
            UpgradableItemREF.SetExpAmount(PreviewUpgradeEXP);
            StartCoroutine(UpgradeProgress());
        }
    }

    private void AutoAdd()
    {
        if (!isMaxError())
        {
            GetRandomsItemButton();
        }
        else
        {
            DisplayMaxError();
            return;
        }

        if (NoofItemsAdded <= 0 && FindAvailableSlot())
            AssetManager.GetInstance().OpenPopupPanel("No Consumable Found");

        DisplayFullError();
    }

    private void DisplayFullError()
    {
        if (!FindAvailableSlot())
            AssetManager.GetInstance().OpenPopupPanel("Slots Full");
    }

    private bool isMaxError()
    {
        UpgradableItems upgradableItems = GetItemREF() as UpgradableItems;
        if (upgradableItems != null)
        {
            if (upgradableItems.GetLevel() + GetLevelIncrease() == CostList.Length)
                return true;
        }

        return false;
    }

    private void DisplayMaxError()
    {
        AssetManager.GetInstance().OpenPopupPanel("Item Max");
    }

    private void UpgradeItem()
    {
        if (GetNoofSlotsTaken() == 0)
        {
            AssetManager.GetInstance().OpenPopupPanel("Select EXP materials");
            return;
        }

        EnhanceUpgrade();

        for (int i = 0; i < GetEnhancementItemList().Count; i++)
        {
            Slot slot = GetEnhancementItemList()[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
            {
                if (slot.GetItemButton().GetItemREF() != null)
                {
                    InventoryManager.GetInstance().RemoveItems(slot.GetItemButton().GetItemREF());
                    slot.SetItemButton(null);
                }
            }
        }
    }

    private void GetRandomsItemButton()
    {
        List<Item> itemsList = new List<Item>(InventoryManager.GetInstance().GetINVList());

        for (int i = 0; i < itemsList.Count; i++)
        {
            Item item = itemsList[i];

            if (item.GetItemSO().GetType() != GetItemREF().GetItemSO().GetType() || (int)item.GetRarity() > RaritySelection)
            {
                itemsList.Remove(item);
                i--;
            }
        }


        NoofItemsAdded = 0;
        int slotsTaken = GetNoofSlotsTaken();

        if ((GetNoofSlotsTaken() >= GetTotalSlots()) || itemsList.Count == 0 || !FindAvailableSlot() || isMaxError())
            return;

        itemsList = itemsList.OrderBy(item => UnityEngine.Random.value).OrderBy(item => (int)item.GetRarity()).ToList();

        for (int i = 0; i < itemsList.Count; i++)
        {
            Item item = itemsList[i];

            if (GetNoofSlotsTaken() >= GetTotalSlots() || isMaxError())
                break;

            if (CheckIfItemalreadyExist(item) != null || item == GetItemREF())
                continue;

            UpgradableItems upgradableItems = item as UpgradableItems;
            if (upgradableItems != null)
            {
                if (upgradableItems.GetLockStatus())
                    continue;

                if (upgradableItems is Artifacts)
                {
                    Artifacts artifacts = upgradableItems as Artifacts;
                    if (artifacts.GetCharacterEquipped() != null)
                        continue;
                }
            }


            FindAvailableSlot()?.SetItemButton(item);
            NoofItemsAdded++;
            slotsTaken++;
        }
    }
}
