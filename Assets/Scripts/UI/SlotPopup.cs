using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SendSlotInfo : EventArgs
{
    public Slot slot;
    public ItemButton itemButtonREF;
}

public class SlotPopup : MonoBehaviour
{
    [SerializeField] GameObject UI_GO;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] ItemContentManager ItemInfoContent;
    public event EventHandler<SendSlotInfo> onSlotSend, onSlotItemRemove;
    [SerializeField] bool AllowDragnDrop;
    private Slot SlotREF;
    private List<ItemButton> itembuttonlist = new();
    // Update is called once per frame
    void Start()
    {
        InventoryManager.GetInstance().onInventoryListChanged += OnInventoryListChanged;
        OnInventoryListChanged();
    }

    public List<ItemButton> GetItemButtonList()
    {
        return itembuttonlist;
    }

    public void OnInventoryListChanged()
    {
        HideItem(null, true);

        for (int i = 0; i < InventoryManager.GetInstance().GetINVList().Count; i++)
        {
            Item item = InventoryManager.GetInstance().GetINVList()[i];

            GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, scrollRect.content);
            ItemButton itemButton = go.GetComponent<ItemButton>();
            itemButton.SetItemsSO(item.GetItemSO());
            itemButton.SetItemREF(item);

            itemButton.onButtonSpawn += OnItemSpawned;
            itemButton.onButtonClick += GetItemSelected;
            itemButton.onButtonRemoveClick += OnItemRemove;

            if (AllowDragnDrop)
            {
                DragnDrop dragnDrop = itemButton.GetComponent<DragnDrop>();
                dragnDrop.parentTransform = transform;
                dragnDrop.onBeginDragEvent += OnBeginDrag;
                dragnDrop.onDragEvent += OnDrag;
                dragnDrop.onEndDragEvent += OnEndDrag;
            }
            itembuttonlist.Add(itemButton);
        }
    }
    private void OnItemSpawned(ItemButton itemButton)
    {
        itemButton.DisableNewImage();
    }

    private void OnBeginDrag(PointerEventData eventData, Transform parentTransform)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;

        ItemInfoContent.TogglePopup(false);
        itemButton.OnBeginDrag(eventData, parentTransform);
    }
    private void OnDrag(PointerEventData eventData)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;
        itemButton.OnDrag(eventData);
    }
    private void OnEndDrag(PointerEventData eventData)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;
        itemButton.OnEndDrag(eventData);
    }

    public void HideItem(Item item, bool hideall = false)
    {
        foreach (RectTransform rt in scrollRect.content)
        {
            ItemButton itemButton = rt.GetComponent<ItemButton>();
            DragnDrop dragnDrop = itemButton.GetComponent<DragnDrop>();
            if (itemButton.GetItemREF() == item || hideall)
            {
                itemButton.onButtonClick -= GetItemSelected;
                itemButton.onButtonSpawn -= OnItemSpawned;
                itemButton.onButtonRemoveClick -= OnItemRemove;

                if (AllowDragnDrop)
                {
                    dragnDrop.onBeginDragEvent -= OnBeginDrag;
                    dragnDrop.onDragEvent -= OnDrag;
                    dragnDrop.onEndDragEvent -= OnEndDrag;
                }
                itembuttonlist.Remove(itemButton);
                Destroy(itemButton.gameObject);
            }
        }
    }

    public ItemButton GetItemButton(Item item)
    {
        foreach (RectTransform rt in scrollRect.content)
        {
            ItemButton itemButton = rt.GetComponent<ItemButton>();
            if (itemButton.GetItemREF() == item)
            {
                return itemButton;
            }
        }
        return null;
    }

    private void OnItemRemove(ItemButton itemButton)
    {
        ItemInfoContent.TogglePopup(false);
        if (SlotREF == null)
            return;

        AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        onSlotItemRemove?.Invoke(this, new SendSlotInfo { slot = SlotREF, itemButtonREF = itemButton });
    }

    private void GetItemSelected(ItemButton itemButton)
    {
        ItemInfoContent.TogglePopup(true);
        ItemInfoContent.SetItemREF(itemButton.GetItemREF(), itemButton.GetItemsSO());
        UpdateOutlineSelection(itemButton);

        if (SlotREF == null)
            return;

        onSlotSend?.Invoke(this, new SendSlotInfo { slot = SlotREF, itemButtonREF = itemButton });
    }

    public void SubscribeSlot(Slot Slot)
    {
        Slot.onSlotClick += OnSlotClick;
    }

    public void UnSubscribeSlot(Slot Slot)
    {
        Slot.onSlotClick -= OnSlotClick;
    }

    private void OnSlotClick(Slot slot)
    {
        SlotREF = slot;

        if (SlotREF.GetItemButton())
        {
            ItemInfoContent.SetItemREF(SlotREF.GetItemButton().GetItemREF(), SlotREF.GetItemButton().GetItemREF().GetItemSO());
            UpdateOutlineSelection(GetItemButton(SlotREF.GetItemButton().GetItemREF()));
            ItemInfoContent.TogglePopup(true);
        }
        TogglePopup(true);
    }

    private void UpdateOutlineSelection(ItemButton selecteditemButton)
    {
        foreach (ItemButton itemButton in itembuttonlist)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }

        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, selecteditemButton);
    }

    private void TogglePopup(bool active)
    {
        UI_GO.SetActive(active);
    }
}
