using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteract
{
    [SerializeField] Sprite ShopIcon;
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

    public void OnInteractUpdate(IInteract interactComponent)
    {
    }
    public void OnInteractExit(IInteract interactComponent)
    {
    }

    public Sprite GetInteractionSprite()
    {
        return ShopIcon;
    }
}
