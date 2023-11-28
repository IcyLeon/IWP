using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] ShopItemSO[] BuffShopItemList;
    [SerializeField] ShopItemSO[] ConsumableShopItemList;
    [SerializeField] TextMeshProUGUI PreviewCostTxt;
    [SerializeField] Image CostCurrencyImage;
    [SerializeField] ItemContentManager ItemContentManager;
    [SerializeField] ShopTabContentManager tabContentManager;
    [SerializeField] Purchase PurchaseItem;
    private ShopInfo selectedShopInfo;

    // Start is called before the first frame update
    void Start()
    {
        foreach(ShopItemSO shopItemSO in BuffShopItemList)
        {
            ShopInfo si = Instantiate(AssetManager.GetInstance().ShopInfoPrefab, tabContentManager.GetTabContent()[0].transform).GetComponent<ShopInfo>();
            si.SetShopItemSO(shopItemSO);
            si.OnShopButtonClick += SelectedShopInfo;
        }
        foreach (ShopItemSO shopItemSO in ConsumableShopItemList)
        {
            ShopInfo si = Instantiate(AssetManager.GetInstance().ShopInfoPrefab, tabContentManager.GetTabContent()[1].transform).GetComponent<ShopInfo>();
            si.SetShopItemSO(shopItemSO);
            si.OnShopButtonClick += SelectedShopInfo;
        }
    }

    private void SelectedShopInfo(ShopInfo shopInfo)
    {
        selectedShopInfo = shopInfo;
        if (selectedShopInfo == null)
            return;

        CostCurrencyImage.sprite = AssetManager.GetInstance().GetCurrencySprite(selectedShopInfo.GetShopItemSO().CurrencyType);
        PurchaseItem.SetShopItemREF(shopInfo.GetShopItemSO());
        ItemContentManager.SetItemREF(null, shopInfo.GetShopItemSO().ItemsSO);
        PreviewCostTxt.text = selectedShopInfo.GetShopItemSO().Price.ToString();
    }

}
