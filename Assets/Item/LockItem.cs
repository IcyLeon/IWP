using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockItem : MonoBehaviour
{
    private Item ItemREF;
    [SerializeField] Image LockImage;

    [Serializable]
    public class LockInfo
    {
        public Color LockBackgroundColor;
        public Color LockImageColor;
        public Sprite LockImage;
    }
    [SerializeField] LockInfo LockedInfo;
    [SerializeField] LockInfo UnLockedInfo;

    public void SetItemREF(Item item)
    {
        ItemREF = item;
        if (ItemREF == null || !(ItemREF is UpgradableItems))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        RefreshLock(ItemREF);
    }
    private void SetLockStatus(UpgradableItems item)
    {
        item.SetLockStatus(!item.GetLockStatus());
        RefreshLock(item);
    }

    public void LockClick()
    {
        if (ItemREF == null)
            return;

        UpgradableItems upgradableItems = ItemREF as UpgradableItems;
        SetLockStatus(upgradableItems);
    }

    private void RefreshLock(Item item)
    {
        UpgradableItems upgradableItems = item as UpgradableItems;

        Image Background = GetComponent<Image>();

        if (upgradableItems.GetLockStatus())
        {
            LockImage.sprite = LockedInfo.LockImage;
            LockImage.color = LockedInfo.LockImageColor;
            Background.color = LockedInfo.LockBackgroundColor;
        }
        else
        {
            LockImage.sprite = UnLockedInfo.LockImage;
            LockImage.color = UnLockedInfo.LockImageColor;
            Background.color = UnLockedInfo.LockBackgroundColor;
        }
    }
}
