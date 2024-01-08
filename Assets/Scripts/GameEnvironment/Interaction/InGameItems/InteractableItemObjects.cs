using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItemObjects : MonoBehaviour, IInteract
{
    private Item ItemREF;

    public void SetItemsREF(Item ItemREF)
    {
        this.ItemREF = ItemREF;
    }


    public bool CanInteract()
    {
        return true;
    }

    public Sprite GetInteractionSprite()
    {
        if (ItemREF == null)
            return null;

        return ItemREF.GetItemSO().ItemSprite;
    }

    private void Start()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Entity"));
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("BossEntity"));
    }

    public void Interact()
    {
        InventoryManager IM = InventoryManager.GetInstance();

        if (ItemREF != null)
        {
            IM.AddItems(ItemREF);
        }

        Destroy(gameObject);
    }

    public string InteractMessage()
    {
        if (ItemREF == null)
            return "Unknown item";

        return ItemREF.GetItemSO().ItemName;
    }

    public void OnInteractExit(IInteract interactComponent)
    {
    }

    public void OnInteractUpdate(IInteract interactComponent)
    {
    }
}
