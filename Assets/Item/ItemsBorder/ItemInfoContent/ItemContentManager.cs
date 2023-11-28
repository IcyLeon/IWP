using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentManager : MonoBehaviour
{
    [SerializeField] Image ItemSprite;
    [SerializeField] Image ItemCardImage;
    [SerializeField] ItemContentDisplay ItemContentDisplay;
    [SerializeField] LockItem LockButton;
    private Item ItemREF;
    private ItemTemplate ItemsSO;

    private void Start()
    {
        if (ItemREF == null && ItemsSO == null)
            TogglePopup(false);
    }

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
        if (ItemREF != null)
        {
            ItemCardImage.sprite = AssetManager.GetInstance().GetItemListTemplate().raritylist[(int)ItemREF.GetRarity()].ItemCardImage;
        }
        else
        {
            ItemCardImage.sprite = AssetManager.GetInstance().GetItemListTemplate().raritylist[(int)ItemsSO.Rarity].ItemCardImage;
        }

        ItemSprite.sprite = ItemsSO.ItemSprite;
        ItemContentDisplay.RefreshItemContentDisplay(ItemREF, ItemsSO);
        LockButton.SetItemREF(ItemREF);

        TogglePopup(ItemREF != null || ItemsSO != null);
    }
    public void TogglePopup(bool active)
    {
        gameObject.SetActive(active);
    }
}
