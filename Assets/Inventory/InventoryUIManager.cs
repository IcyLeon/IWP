using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] ItemContentManager ItemContentManager;
    [SerializeField] GameObject ItemContent;
    [SerializeField] UseItem UseItem;
    [SerializeField] InvTabGroup TabGroup;
    [SerializeField] ScrollRect ScrollRect;
    private InventoryManager InventoryManager;
    private List<ItemButton> itembuttonlist;
    private ItemButton SelectedItemButton;
    private Item selectedItem;

    // Start is called before the first frame update
    void Start()
    {
        itembuttonlist = new();
        InventoryManager = InventoryManager.GetInstance();
        InventoryManager.onInventoryListChanged += OnItemChanged;
        TabGroup.onTabChanged += onTabChangedEvent;

        OnItemChanged();
    }

    private void OnDestroy()
    {
        InventoryManager.onInventoryListChanged -= OnItemChanged;
        TabGroup.onTabChanged -= onTabChangedEvent;
    }

    private void onTabChangedEvent(object sender, EventArgs e)
    {
        ScrollRect.content = TabGroup.GetCurrentTabPanel().TabPanel.GetComponent<RectTransform>();
    }

    void OnItemChanged()
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

            GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab);
            ItemButton itemButton = go.GetComponent<ItemButton>();
            itemButton.SetItemsSO(item.GetItemSO());
            itemButton.SetItemREF(item);
            itemButton.onButtonUpdate += GetItemButtonUpdate;


            itemButton.transform.SetParent(TabGroup.GetTabMenuList()[TabGroup.GetTabPanelIdx(itemButton.GetItemsSO().GetCategory())].TabPanel.transform);

            itemButton.onButtonClick += GetItemSelected;
            itembuttonlist.Add(itemButton);
        }
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

    void GetItemSelected(ItemButton itemButton)
    {
        if (itemButton == null)
            return;
        selectedItem = itemButton.GetItemREF();
        ItemContentManager.SetItemREF(selectedItem, itemButton.GetItemsSO());
        UpdateOutlineSelection();
    }

    private ItemButton GetItemButton(Item item)
    {
        foreach (ItemButton itemButton in itembuttonlist)
        {
            if (itemButton.GetItemREF() == item)
                return itemButton;
        }

        return null;
    }

    private void UpdateOutlineSelection()
    {
        foreach (ItemButton itemButton in itembuttonlist)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }
        SelectedItemButton = GetItemButton(selectedItem);
        UseItem.SetItemREF(SelectedItemButton);
        ItemContent.SetActive(SelectedItemButton != null);
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, SelectedItemButton);
    }
}
