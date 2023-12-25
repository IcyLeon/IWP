using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] ItemContentManager ItemContentManager;
    [SerializeField] GameObject ItemContent;
    [SerializeField] UpgradeCanvasTransition UpgradeItem;
    [SerializeField] UseItem UseItem;
    [SerializeField] InvTabGroup TabGroup;
    [SerializeField] ScrollRect ScrollRect;
    private InventoryManager InventoryManager;
    private Dictionary<Item, ItemButton> itembutton_Dictionary;
    private ItemButton SelectedItemButton;
    private Item selectedItem;

    // Start is called before the first frame update
    void Start()
    {
        itembutton_Dictionary = new();
        InventoryManager = InventoryManager.GetInstance();
        InventoryManager.OnInventoryItemAdd += OnInventoryItemAdd;
        InventoryManager.OnInventoryItemRemove += OnInventoryItemRemove;
        TabGroup.onTabChanged += onTabChangedEvent;

        Init();
    }

    private void OnDestroy()
    {
        for (int i = itembutton_Dictionary.Count - 1; i > 0; i--)
        {
            KeyValuePair<Item, ItemButton> itemPair = itembutton_Dictionary.ElementAt(i);
            if (itembutton_Dictionary.TryGetValue(itemPair.Key, out ItemButton itemButton))
            {
                itemButton.onButtonClick -= GetItemSelected;
                itemButton.onButtonUpdate -= GetItemButtonUpdate;
                Destroy(itemButton.gameObject);
                itembutton_Dictionary.Remove(itemPair.Key);
            }
        }
        InventoryManager.OnInventoryItemAdd -= OnInventoryItemAdd;
        InventoryManager.OnInventoryItemRemove -= OnInventoryItemRemove;
        TabGroup.onTabChanged -= onTabChangedEvent;
    }


    private void onTabChangedEvent(object sender, EventArgs e)
    {
        ScrollRect.content = TabGroup.GetCurrentTabPanel().TabPanel.GetComponent<RectTransform>();
    }

    private void OnInventoryItemAdd(Item item)
    {
        GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab);
        ItemButton itemButton = go.GetComponent<ItemButton>();
        itemButton.SetItemsSO(item.GetItemSO());
        itemButton.SetItemREF(item);
        itemButton.onButtonUpdate += GetItemButtonUpdate;
        itemButton.onButtonClick += GetItemSelected;
        itemButton.transform.SetParent(TabGroup.GetTabMenuList()[TabGroup.GetTabPanelIdx(itemButton.GetItemsSO().GetCategory())].TabPanel.transform);
        itembutton_Dictionary.Add(item, itemButton);
    }

    private void OnInventoryItemRemove(Item item)
    {
        ItemButton itemButton = itembutton_Dictionary[item];
        itemButton.onButtonClick -= GetItemSelected;
        itemButton.onButtonUpdate -= GetItemButtonUpdate;
        Destroy(itemButton.gameObject);
        itembutton_Dictionary.Remove(item);
    }


    void Init()
    {
        for (int i = 0; i < InventoryManager.GetINVList().Count; i++)
        {
            Item item = InventoryManager.GetINVList()[i];
            OnInventoryItemAdd(item);
        }
    }

    private void Update()
    {
        ItemContent.SetActive(SelectedItemButton != null);
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


    private void UpdateOutlineSelection()
    {
        foreach (ItemButton itemButton in itembutton_Dictionary.Values)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }
        SelectedItemButton = itembutton_Dictionary[selectedItem];
        UpgradeItem.SetItemREF(SelectedItemButton);
        UseItem.SetItemREF(SelectedItemButton);
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, SelectedItemButton);
    }
}