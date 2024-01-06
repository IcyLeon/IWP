using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItemObjects : MonoBehaviour, IInteract
{
    private ItemTemplate itemsSO;

    public void SetItemsSO(ItemTemplate itemTemplate)
    {
        itemsSO = itemTemplate;
    }

    public bool CanInteract()
    {
        return true;
    }

    public Sprite GetInteractionSprite()
    {
        if (itemsSO == null)
            return null;

        return itemsSO.ItemSprite;
    }

    public void Interact()
    {
        if (itemsSO == null)
            return;

        Type ItemType = itemsSO.GetTypeREF();
        object instance = Activator.CreateInstance(ItemType, true, itemsSO);
        Item item = (Item)instance;
        InventoryManager.GetInstance().AddItems(item);

        Destroy(gameObject);
    }

    public string InteractMessage()
    {
        if (itemsSO == null)
            return "Unknown item";

        return itemsSO.ItemName;
    }

    public void OnInteractExit(IInteract interactComponent)
    {
    }

    public void OnInteractUpdate(IInteract interactComponent)
    {
    }
}
