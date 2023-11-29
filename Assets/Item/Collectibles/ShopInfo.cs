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
    [SerializeField] Image BackgroundImage;
    [SerializeField] Color32 DefaultColor;
    [SerializeField] Color32 SelectedColor;
    public delegate void OnShopItemClick(ShopInfo shopInfo);
    public OnShopItemClick OnShopButtonClick;
    private ShopItemSO ShopItemSO;

    public void SetShopItemSO(ShopItemSO itemSO)
    {
        ShopItemSO = itemSO;
        UpdateContent();
    }

    public void UpdateBackground(ShopInfo shopInfo)
    {
        if (shopInfo == this)
        {
            BackgroundImage.color = SelectedColor;

            Color color = new Color32(85, 93, 105, 255);
            color.a = 1f;
            ShopItemTxt.color = color;
        }
        else
        {
            BackgroundImage.color = DefaultColor;

            Color color = SelectedColor;
            color.a = 1f;
            ShopItemTxt.color = color;
        }
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
