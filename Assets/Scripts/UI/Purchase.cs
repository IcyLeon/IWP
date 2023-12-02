using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public class Purchase : MonoBehaviour, IPointerClickHandler
{
    private InventoryManager inventoryManager;
    private ShopItemSO ShopItemSOREF;

    public void Start()
    {
        inventoryManager = InventoryManager.GetInstance();
    }
    public void SetShopItemREF(ShopItemSO shopItemSO)
    {
        ShopItemSOREF = shopItemSO;
    }

    private void PurchaseItem()
    {
        if (ShopItemSOREF == null)
            return;

        if (CanBuy(ShopItemSOREF))
        {
            inventoryManager.RemoveCurrency(ShopItemSOREF.CurrencyType, ShopItemSOREF.Price);
            AddItem(ShopItemSOREF.ItemsSO);
            AssetManager.GetInstance().OpenPopupPanel("Purchase Completed!");
        }
        else
        {
            AssetManager.GetInstance().OpenPopupPanel("Insufficient Fund!");
        }
    }

    private void AddItem(ItemTemplate itemsSO)
    {
        Type ItemType = itemsSO.GetTypeREF();
        object instance = Activator.CreateInstance(ItemType, true, itemsSO);
        Item item = (Item)instance;
        inventoryManager.AddItems(item);
    }

    private bool CanBuy(ShopItemSO s)
    {
        if (s == null)
            return true;

        return InventoryManager.GetInstance().GetCurrency(s.CurrencyType) >= s.Price;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PurchaseItem();
    }
}
