using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    private InventoryManager InventoryManager;
    private Dictionary<Item, ItemButton> itembutton_Dictionary = new();

    // Update is called once per frame
    void Start()
    {
        InventoryManager = InventoryManager.GetInstance();
        InventoryManager.OnInventoryItemAdd += OnInventoryItemAdd;
        InventoryManager.OnInventoryItemRemove += OnInventoryItemRemove;
        Init();
    }

    public Dictionary<Item, ItemButton> GetItemButton_Dictionary()
    {
        return itembutton_Dictionary;
    }

    void Init()
    {
        for (int i = 0; i < InventoryManager.GetINVList().Count; i++)
        {
            Item item = InventoryManager.GetINVList()[i];
            OnInventoryItemAdd(item, item.GetItemSO());
        }
    }

    private void OnDestroy()
    {
        for (int i = itembutton_Dictionary.Count - 1; i > 0; i--)
        {
            KeyValuePair<Item, ItemButton> itemPair = itembutton_Dictionary.ElementAt(i);
            if (itembutton_Dictionary.TryGetValue(itemPair.Key, out ItemButton itemButton))
            {
                DragnDropButton dragnDrop = itemButton.GetComponent<DragnDropButton>();
                itemButton.onButtonSpawn -= OnItemSpawned;
                itemButton.onButtonRemoveClick -= OnItemRemove;
                itemButton.onButtonClick -= GetItemSelected;
                dragnDrop.onBeginDragEvent -= OnBeginDrag;
                dragnDrop.onDragEvent -= OnDrag;
                dragnDrop.onEndDragEvent -= OnEndDrag;
                Destroy(itemButton.gameObject);
                itembutton_Dictionary.Remove(itemPair.Key);
            }
        }

        InventoryManager.OnInventoryItemAdd -= OnInventoryItemAdd;
        InventoryManager.OnInventoryItemRemove -= OnInventoryItemRemove;
    }

    private void OnInventoryItemAdd(Item item, ItemTemplate itemSO)
    {
        GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, scrollRect.content);
        ItemButton itemButton = go.GetComponent<ItemButton>();
        itemButton.SetItemsSO(item.GetItemSO());
        itemButton.SetItemREF(item);
        itemButton.onButtonSpawn += OnItemSpawned;
        itemButton.onButtonRemoveClick += OnItemRemove;
        itemButton.onButtonClick += GetItemSelected;
        if (AllowDragnDrop)
        {
            DragnDropButton dragnDrop = itemButton.GetComponent<DragnDropButton>();
            dragnDrop.parentTransform = transform;
            dragnDrop.onBeginDragEvent += OnBeginDrag;
            dragnDrop.onDragEvent += OnDrag;
            dragnDrop.onEndDragEvent += OnEndDrag;
        }
        itembutton_Dictionary.Add(item, itemButton);
    }

    private void OnInventoryItemRemove(Item item, ItemTemplate itemSO)
    {
        ItemButton itemButton = itembutton_Dictionary[item];
        itemButton.onButtonSpawn -= OnItemSpawned;
        itemButton.onButtonRemoveClick -= OnItemRemove;
        itemButton.onButtonClick -= GetItemSelected;
        DragnDropButton dragnDrop = itemButton.GetComponent<DragnDropButton>();
        dragnDrop.onBeginDragEvent -= OnBeginDrag;
        dragnDrop.onDragEvent -= OnDrag;
        dragnDrop.onEndDragEvent -= OnEndDrag;
        Destroy(itemButton.gameObject);
        itembutton_Dictionary.Remove(item);
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

    public void HideItem(Item item)
    {
        if (itembutton_Dictionary == null)
            return;

        foreach (var rt in itembutton_Dictionary.Values)
        {
            if (rt.GetItemREF() == item)
                rt.gameObject.SetActive(false);
        }
    }

    public void Refresh()
    {
        if (itembutton_Dictionary == null)
            return;

        foreach (var rt in itembutton_Dictionary.Values)
        {
            rt.gameObject.SetActive(true);
        }
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
            UpdateOutlineSelection(itembutton_Dictionary[SlotREF.GetItemButton().GetItemREF()]);
            ItemInfoContent.TogglePopup(true);
        }
        TogglePopup(true);
    }

    private void UpdateOutlineSelection(ItemButton selecteditemButton)
    {
        foreach (ItemButton itemButton in itembutton_Dictionary.Values)
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
