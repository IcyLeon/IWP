using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerExitHandler, IDropHandler, IPointerEnterHandler, IToggle
{
    [SerializeField] GameObject slotItemButtonPrefab;
    [SerializeField] GameObject ContentContainer;
    [SerializeField] Image OutlineAnimation, Outline;
    private ItemButton itembutton;

    public delegate void SendSlotClick(Slot slot);
    public event SendSlotClick onSlotClick;

    public delegate void onSlotItemButtonChanged(SendSlotInfo sendslotInfo, Item item);
    public event onSlotItemButtonChanged SlotItemButtonChanged;

    public void SetItemButton(Item item)
    {
        if (itembutton != null)
        {
            if (itembutton.GetItemREF() != null && item != null)
                return;
        }
        else
        {
            itembutton = Instantiate(slotItemButtonPrefab, transform).GetComponent<ItemButton>();
            itembutton.onButtonSpawn += OnItemSpawned;
        }

        itembutton.CanvasGroup.blocksRaycasts = false;
        if (item != null)
            itembutton.SetItemREF(item);

        SlotItemButtonChanged?.Invoke(
            new SendSlotInfo
            {
                slot = this,
                itemButtonREF = itembutton
            },
            item
        );

        if (item == null)
        {
            itembutton.onButtonSpawn -= OnItemSpawned;
            Destroy(itembutton.gameObject);
        }
    }
    private void OnItemSpawned(ItemButton itemButton)
    {
        itemButton.DisableNewImage();
    }

    private void Update()
    {
        if (OutlineAnimation.gameObject.activeSelf)
        {
            transform.localScale = new Vector3(1.02f, 1.02f, 1f);
            Outline.gameObject.SetActive(true);
        }
    }

    public ItemButton GetItemButton()
    {
        return itembutton;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        onSlotClick?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.95f, 0.95f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        Outline.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;

        SetItemButton(itemButton.GetItemREF());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.02f, 1.02f, 1f);
        Outline.gameObject.SetActive(true);
    }

    void IToggle.ToggleSelection(bool toggle)
    {
        OutlineAnimation.gameObject.SetActive(toggle);
        Outline.gameObject.SetActive(toggle);

        if (!toggle)
        {
            transform.localScale = Vector3.one;
        }
    }
}
