using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Image SelectedItemImage;
    [SerializeField] ItemContentDisplay ItemContentDisplay;

    [Header("Display Artifacts")]
    [SerializeField] ArtifactTabGroup TabGroup;
    private Item SelectedItem, PreviousSelectedItem;
    private ItemTemplate SelectedItemsSO;
    private ItemButton SelectedItemButton;
    private List<ItemButton> itembuttonlist = new();

    // Start is called before the first frame update
    void Start()
    {
        InventoryManager.GetInstance().onInventoryListChanged += OnInventoryListChanged;
        OnInventoryListChanged();
        TabGroup.onTabChanged += onTabChangedEvent;

        DetailsPanel.SetActive(false);
    }

    private void Update()
    {
        UpdateArtifactEquipContent();
    }

    private void UpdateArtifactEquipContent()
    {
        Artifacts artifacts = SelectedItem as Artifacts;
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
        if (SelectedItem != null)
            SetCurrentBackground(SelectedItem.GetRarity());
        else
            SetCurrentBackground(SelectedItemsSO.Rarity);

        SelectedItemImage.sprite = SelectedItemsSO.ItemSprite;
        EquipButton.SetItemREF(SelectedItem);
        LockButton.SetItemREF(SelectedItem);
        ItemContentDisplay.RefreshItemContentDisplay(SelectedItem, SelectedItemsSO);
    }

    private void onTabChangedEvent(object sender, EventArgs e)
    {
        ScrollRect.content = TabGroup.GetCurrentTabPanel().TabPanel.GetComponent<RectTransform>();
    }

    private ItemButton GetItemButton(Item item)
    {
        foreach(ItemButton itemButton in itembuttonlist)
        {
            if (itemButton.GetItemREF() == item)
                return itemButton;
        }

        return null;
    }
    private void OnDestroy()
    {
        InventoryManager.GetInstance().onInventoryListChanged -= OnInventoryListChanged;
        TabGroup.onTabChanged -= onTabChangedEvent;
    }
    private void OnInventoryListChanged()
    {

        foreach (ItemButton itemButton in itembuttonlist)
        {
            if (itemButton != null)
            {
                itemButton.onButtonClick -= GetItemSelected;
                itemButton.onButtonUpdate -= GetItemButtonUpdate;
                Destroy(itemButton.gameObject);
            }
        }
        itembuttonlist.Clear();

        for (int i = 0; i < InventoryManager.GetInstance().GetINVList().Count; i++)
        {
            Item item = InventoryManager.GetInstance().GetINVList()[i];
            if (item is not UpgradableItems)
                continue;

            ItemButton itemButton = Instantiate(AssetManager.GetInstance().ItemBorderPrefab).GetComponent<ItemButton>();
            itemButton.SetItemsSO(item.GetItemSO());
            itemButton.SetItemREF(item);
            itemButton.onButtonUpdate += GetItemButtonUpdate;

            UpgradableItems UpgradableItemREF = itemButton.GetItemREF() as UpgradableItems;
            if (UpgradableItemREF != null)
            {
                UpgradableItemREF.onLevelChanged += onItemUpgrade;
            }
            switch (itemButton.GetItemsSO().GetCategory())
            {
                case Category.ARTIFACTS:
                    Artifacts artifacts = UpgradableItemREF as Artifacts;
                    itemButton.gameObject.transform.SetParent(TabGroup.GetTabMenuList()[TabGroup.GetTabPanelIdx(artifacts.GetArtifactType())].TabPanel.transform);
                    break;
            }
            itemButton.onButtonClick += GetItemSelected;
            itembuttonlist.Add(itemButton);
        }

        UpdateOutlineSelection();
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

    private void GetItemSelected(ItemButton itemButton)
    {
        PreviousSelectedItem = SelectedItem;
        SelectedItemsSO = itemButton.GetItemsSO();
        SelectedItem = itemButton.GetItemREF();
        UpdateOutlineSelection();

        DetailsPanel.SetActive(SelectedItemsSO != null);

        if (SelectedItemsSO == null)
            return;

        DisplaySelectedItem();
    }

    private void onItemUpgrade()
    {
        DisplaySelectedItem();
    }
    public Item GetItemCurrentSelected()
    {
        return SelectedItem;
    }

    private void UpdateOutlineSelection()
    {
        foreach (ItemButton itemButton in itembuttonlist)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }
        SelectedItemButton = GetItemButton(SelectedItem);
        UpgradeButton.GetComponent<UpgradeCanvasTransition>().SetItemREF(SelectedItem);
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, SelectedItemButton);
    }
}
