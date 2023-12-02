using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteract
{
    [SerializeField] GameObject ShopUI;
    public void Interact()
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
}
