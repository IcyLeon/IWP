using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentManager : MonoBehaviour
{
    [SerializeField] Image ItemSprite;
    [SerializeField] Image ItemCardImage;
    [SerializeField] GameObject[] ItemContent;
    [SerializeField] ItemContentDisplay ItemContentDisplay;
    [SerializeField] LockItem LockButton;
    private Item ItemREF;
    private ItemTemplate ItemsSO;

    public void SetItemREF(Item item, ItemTemplate itemsSO)
    {
        ItemREF = item;
        ItemsSO = itemsSO;

        UpgradableItems upgradableItems = ItemREF as UpgradableItems;
        if (upgradableItems != null)
        {
            upgradableItems.onLevelChanged += onUpgradeLevelChanged;
        }
        DisplayContent();
    }

    private void onUpgradeLevelChanged()
    {
        DisplayContent();
    }
    private void DisplayContent()
    {
        foreach (GameObject go in ItemContent)
            go.SetActive(false);

        if (ItemREF != null)
        {
            ItemCardImage.sprite = AssetManager.GetInstance().GetItemListTemplate().raritylist[(int)ItemREF.GetRarity()].ItemCardImage;
            if (ItemREF is UpgradableItems)
                ItemContent[0].SetActive(true);
            else if (ItemREF is ConsumableItem)
                ItemContent[1].SetActive(true);
        }
        else
        {
            ItemCardImage.sprite = AssetManager.GetInstance().GetItemListTemplate().raritylist[(int)ItemsSO.Rarity].ItemCardImage;
        }

        ItemSprite.sprite = ItemsSO.ItemSprite;
        ItemContentDisplay.RefreshItemContentDisplay(ItemREF, ItemsSO);
        LockButton.SetItemREF(ItemREF);
    }
    public void TogglePopup(bool active)
    {
        gameObject.SetActive(active);
    }
}
