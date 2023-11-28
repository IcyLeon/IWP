using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopInfo : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image ShopIcon;
    [SerializeField] Image CostImage;
    [SerializeField] TextMeshProUGUI ShopItemTxt;
    [SerializeField] TextMeshProUGUI PriceTxt;
    public delegate void OnShopItemClick(ShopInfo shopInfo);
    public OnShopItemClick OnShopButtonClick;
    private ShopItemSO ShopItemSO;

    public void SetShopItemSO(ShopItemSO itemSO)
    {
        ShopItemSO = itemSO;
        UpdateContent();
    }

    public void UpdateContent()
    {
        if (ShopItemSO == null)
            return;

        string name = ShopItemSO.ItemsSO.ItemName;
        if (ShopItemSO.ItemsSO is ArtifactsSO)
        {
            ArtifactsSO artifactsSO = (ArtifactsSO)ShopItemSO.ItemsSO;
            name = artifactsSO.GetItemType();
        }
        ShopIcon.sprite = ShopItemSO.ItemsSO.ItemSprite;
        ShopItemTxt.text = name;
        CostImage.sprite = AssetManager.GetInstance().GetCurrencySprite(ShopItemSO.CurrencyType);
        PriceTxt.text = ShopItemSO.Price.ToString();
    }

    public ShopItemSO GetShopItemSO()
    {
        return ShopItemSO;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnShopButtonClick?.Invoke(this);
    }
}
