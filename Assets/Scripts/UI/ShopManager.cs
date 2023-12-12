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
    [SerializeField] GameObject ItemContent;
    [SerializeField] ShopTabContentManager tabContentManager;
    [SerializeField] Purchase PurchaseItem;
    private ShopInfo selectedShopInfo;
    private List<ShopInfo> ShopInfoList;

    // Start is called before the first frame update
    void Start()
    {
        ShopInfoList = new();
        foreach (ShopItemSO shopItemSO in BuffShopItemList)
        {
            ShopInfo si = Instantiate(AssetManager.GetInstance().ShopInfoPrefab, tabContentManager.GetTabContent()[0].transform).GetComponent<ShopInfo>();
            si.SetShopItemSO(shopItemSO);
            si.OnShopButtonClick += SelectedShopInfo;
            si.OnShopButtonClick += SelectedColor_Buff;
            ShopInfoList.Add(si);
        }
        foreach (ShopItemSO shopItemSO in ConsumableShopItemList)
        {
            ShopInfo si = Instantiate(AssetManager.GetInstance().ShopInfoPrefab, tabContentManager.GetTabContent()[1].transform).GetComponent<ShopInfo>();
            si.SetShopItemSO(shopItemSO);
            si.OnShopButtonClick += SelectedShopInfo;
            si.OnShopButtonClick += SelectedColor_Consumable;
            ShopInfoList.Add(si);
        }
        ItemContent.SetActive(false);
    }

    private void SelectedColor_Buff(ShopInfo s)
    {
        foreach (ShopInfo shopInfo in tabContentManager.GetTabContent()[0].GetComponentsInChildren<ShopInfo>())
        {
            shopInfo.UpdateBackground(selectedShopInfo);
        }
    }
    private void SelectedColor_Consumable(ShopInfo s)
    {
        foreach (ShopInfo shopInfo in tabContentManager.GetTabContent()[1].GetComponentsInChildren<ShopInfo>())
        {
            shopInfo.UpdateBackground(selectedShopInfo);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < ShopInfoList.Count; i++)
        {
            ShopInfo si = ShopInfoList[i];
            si.OnShopButtonClick -= SelectedShopInfo;
            si.OnShopButtonClick -= SelectedColor_Buff;
            si.OnShopButtonClick -= SelectedColor_Consumable;
        }
    }
    private void SelectedShopInfo(ShopInfo shopInfo)
    {
        selectedShopInfo = shopInfo;
        if (selectedShopInfo == null)
        {
            ItemContent.SetActive(false);
            return;
        }

        ItemContent.SetActive(true);
        CostCurrencyImage.sprite = AssetManager.GetInstance().GetCurrencySprite(selectedShopInfo.GetShopItemSO().CurrencyType);
        PurchaseItem.SetShopItemREF(shopInfo.GetShopItemSO());
        ItemContentManager.SetItemREF(null, shopInfo.GetShopItemSO().ItemsSO);
        PreviewCostTxt.text = selectedShopInfo.GetShopItemSO().Price.ToString();
    }

}
