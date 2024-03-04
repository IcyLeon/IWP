using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MarkerIndicatorInteraction, IInteract
{
    [SerializeField] GameObject ShopUI;
    public void Interact(PlayerManager PM)
    {
        ShopUI.SetActive(!ShopUI.activeSelf);
    }

    public string InteractMessage()
    {
        return "Shop";
    }

    public bool CanInteract()
    {
        return true;
    }

    public void OnInteractEnter(IInteract interactComponent)
    {
    }
    public void OnInteractExit(IInteract interactComponent)
    {
    }

    public Sprite GetInteractionSprite()
    {
        return GetWorldMarkerSprite();
    }
}
