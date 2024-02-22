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
        //Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Entity"));
        //Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("BossEntity"));
    }

    private void Update()
    {
        UpdateOutofBound();
    }

    private void UpdateOutofBound()
    {
        bool OutOfBound = Characters.isOutofBound(transform.position);
        if (OutOfBound)
        {
            Destroy(gameObject);
        }
    }


    public void Interact(PlayerManager PM)
    {
        InventoryManager IM = InventoryManager.GetInstance();

        if (ItemREF != null)
        {
            if (PM)
                PM.SpawnItemCollectedUI(ItemREF.GetItemSO());
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
