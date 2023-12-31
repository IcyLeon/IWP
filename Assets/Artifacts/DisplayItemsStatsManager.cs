using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class DisplayItemsStatsManager : MonoBehaviour
{
    [System.Serializable]
    public class Background
    {
        public GameObject BackgroundGO;
        public Color ParticlesColor;
        public Rarity Rarity;
    }
    [SerializeField] ParticleSystem fog;
    [SerializeField] Background[] Backgrounds;

    [Header("Display Upgradable in Inventory")]
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] GameObject DetailsPanel;
    [SerializeField] LockItem LockButton;
    [SerializeField] GameObject UpgradeButton;
    [SerializeField] EquipItems EquipButton;
    [SerializeField] EquippedByCharacter EquippedByCharacter;
    [SerializeField] SelectedArtifactsBubble SelectedArtifactsBubble;
    [SerializeField] ItemContentDisplay ItemContentDisplay;

    [Header("Display Artifacts")]
    [SerializeField] ArtifactTabGroup TabGroup;
    private Item SelectedItem, PreviousSelectedItem;
    private ItemTemplate SelectedItemsSO;
    private ItemButton SelectedItemButton;
    private InventoryManager InventoryManager;
    private Dictionary<Item, ItemButton> itembutton_Dictionary;

    // Start is called before the first frame update
    void Start()
    {
        itembutton_Dictionary = new();
        InventoryManager = InventoryManager.GetInstance();
        InventoryManager.OnInventoryItemAdd += OnInventoryItemAdd;
        InventoryManager.OnInventoryItemRemove += OnInventoryItemRemove;
        TabGroup.onTabChanged += onTabChangedEvent;

        DetailsPanel.SetActive(false);
        Init();
    }

    private void OnInventoryItemAdd(Item item, ItemTemplate itemSO)
    {
        GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab);
        ItemButton itemButton = go.GetComponent<ItemButton>();
        itemButton.SetItemsSO(item.GetItemSO());
        itemButton.SetItemREF(item);

        UpgradableItems upgradableItems = item as UpgradableItems;
        if (upgradableItems != null)
        {
            upgradableItems.onLevelChanged += onUpgradeLevelChanged;
        }

        itemButton.onButtonUpdate += GetItemButtonUpdate;
        itemButton.onButtonClick += GetItemSelected;

        switch (itemButton.GetItemsSO().GetCategory())
        {
            case Category.ARTIFACTS:
                Artifacts artifacts = itemButton.GetItemREF() as Artifacts;
                itemButton.gameObject.transform.SetParent(TabGroup.GetTabMenuList()[TabGroup.GetTabMenuByArtifactType(artifacts.GetArtifactType())].TabPanel.transform);
                break;
        }

        itembutton_Dictionary.Add(item, itemButton);
    }

    private void OnInventoryItemRemove(Item item, ItemTemplate itemSO)
    {
        ItemButton itemButton = itembutton_Dictionary[item];
        itemButton.onButtonClick -= GetItemSelected;
        itemButton.onButtonUpdate -= GetItemButtonUpdate;
        UpgradableItems upgradableItems = item as UpgradableItems;
        if (upgradableItems != null)
        {
            upgradableItems.onLevelChanged -= onUpgradeLevelChanged;
        }

        Destroy(itemButton.gameObject);
        itembutton_Dictionary.Remove(item);
    }

    void Init()
    {
        for (int i = 0; i < InventoryManager.GetINVList().Count; i++)
        {
            Item item = InventoryManager.GetINVList()[i];
            OnInventoryItemAdd(item, item.GetItemSO());
        }
    }

    private void Update()
    {
        UpdateArtifactEquipContent();
    }

    private void UpdateArtifactEquipContent()
    {
        Artifacts artifacts = GetItemCurrentSelected() as Artifacts;
        EquippedByCharacter.gameObject.SetActive(false);
        if (artifacts != null)
        {
            if (artifacts.GetCharacterEquipped() != null)
            {
                PlayerCharacterSO playersCharacterSO = artifacts.GetCharacterEquipped().GetItemSO() as PlayerCharacterSO;
                EquippedByCharacter.UpdateContent(playersCharacterSO.PartyIcon, playersCharacterSO.ItemName);
                EquippedByCharacter.gameObject.SetActive(true);
            }
        }
    }
    private void ChangeParticleColor(Color color)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[fog.particleCount];
        int numParticles = fog.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            particles[i].startColor = color;
        }

        fog.SetParticles(particles, numParticles);

        var fogMain = fog.main;
        fogMain.startColor = color;
    }

    void SetCurrentBackground(Rarity rarity)
    {
        for (int i = 0; i < Backgrounds.Length; i++)
            Backgrounds[i].BackgroundGO.SetActive(false);

        Background background = GetBackgroundGO(rarity);
        if (background != null)
        {
            ChangeParticleColor(background.ParticlesColor);
            background.BackgroundGO.SetActive(true);
        }
    }

    private Background GetBackgroundGO(Rarity rarity)
    {
        for (int i = 0; i < Backgrounds.Length; i++)
        {
            if (Backgrounds[i].Rarity == rarity)
            {
                return Backgrounds[i];
            }
        }
        return null;
    }
    // Update is called once per frame
    void DisplaySelectedItem()
    {
        if (GetItemCurrentSelected() != null)
            SetCurrentBackground(GetItemCurrentSelected().GetRarity());
        else
            SetCurrentBackground(SelectedItemsSO.Rarity);

        SelectedArtifactsBubble.UpdateSelectedItem((Artifacts)GetItemCurrentSelected(), (ArtifactsSO)SelectedItemsSO);
        EquipButton.SetItemREF(GetItemCurrentSelected());
        LockButton.SetItemREF(GetItemCurrentSelected());
        ItemContentDisplay.RefreshItemContentDisplay(GetItemCurrentSelected(), SelectedItemsSO);
    }

    private void onTabChangedEvent()
    {
        ScrollRect.content = TabGroup.GetCurrentTabPanel().TabPanel.GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        for (int i = itembutton_Dictionary.Count - 1; i > 0; i--)
        {
            KeyValuePair<Item, ItemButton> itemPair = itembutton_Dictionary.ElementAt(i);
            if (itembutton_Dictionary.TryGetValue(itemPair.Key, out ItemButton itemButton))
            {
                if (itemButton != null)
                {
                    UpgradableItems upgradableItems = itemPair.Key as UpgradableItems;
                    if (upgradableItems != null)
                    {
                        upgradableItems.onLevelChanged -= onUpgradeLevelChanged;
                    }


                    itemButton.onButtonClick -= GetItemSelected;
                    itemButton.onButtonUpdate -= GetItemButtonUpdate;
                    Destroy(itemButton.gameObject);
                    itembutton_Dictionary.Remove(itemPair.Key);
                }
            }
        }

        InventoryManager.OnInventoryItemAdd -= OnInventoryItemAdd;
        InventoryManager.OnInventoryItemRemove -= OnInventoryItemRemove;
        TabGroup.onTabChanged -= onTabChangedEvent;
    }

    private void GetItemButtonUpdate(ItemButton itemButton)
    {
        if (itemButton.GetItemREF() != null)
        {
            switch (itemButton.GetItemsSO().GetCategory())
            {
                case Category.ARTIFACTS:
                    Artifacts artifacts = itemButton.GetItemREF() as Artifacts;
                    if (artifacts != null)
                    {
                        PlayerCharacterSO playersCharacterSO = artifacts.GetCharacterEquipped()?.GetItemSO() as PlayerCharacterSO;
                        if (playersCharacterSO)
                            itemButton.GetEquipByIconContent().UpdateContent(playersCharacterSO.PartyIcon);
                    }
                    itemButton.GetEquipByIconContent().gameObject.SetActive(artifacts?.GetCharacterEquipped() != null);
                    break;
            }
        }
    }

    private void SelectArtifactTabSelectedItem(ItemButton itemButton)
    {
        Artifacts artifacts = (Artifacts)(itemButton.GetItemREF());
        if (artifacts == null)
            return;

        TabGroup.OnTabSelected(TabGroup.GetTabMenuList()[TabGroup.GetTabMenuByArtifactType(artifacts.GetArtifactType())].TabButton);
    }
    private void GetItemSelected(ItemButton itemButton)
    {
        PreviousSelectedItem = GetItemCurrentSelected();

        if (itemButton == null)
            return;

        SelectArtifactTabSelectedItem(itemButton);
        SelectedItemsSO = itemButton.GetItemsSO();
        SelectedItem = itemButton.GetItemREF();
        UpdateOutlineSelection();

        DetailsPanel.SetActive(SelectedItemsSO != null);

        if (SelectedItemsSO == null)
            return;

        DisplaySelectedItem();
        UpdateOutlineSelection();
    }

    private void onUpgradeLevelChanged()
    {
        SetCurrentSelectedItem(SelectedItem);
    }

    public Item GetItemCurrentSelected()
    {
        return SelectedItem;
    }

    public void SetCurrentSelectedItem(Item item)
    {
        SelectedItem = item;
        UpdateOutlineSelection();
        GetItemSelected(SelectedItemButton);
    }

    public void OpenDisplayItemsStatsManager(Item item)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        if (item != null)
        {
            SetCurrentSelectedItem(item);
        }
    }

    private void UpdateOutlineSelection()
    {
        if (GetItemCurrentSelected() == null)
            return;
        foreach (ItemButton itemButton in itembutton_Dictionary.Values)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }
        SelectedItemButton = itembutton_Dictionary[GetItemCurrentSelected()];
        UpgradeButton.GetComponent<UpgradeCanvasTransition>().SetItemREF(SelectedItemButton);
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, SelectedItemButton);
    }
}
