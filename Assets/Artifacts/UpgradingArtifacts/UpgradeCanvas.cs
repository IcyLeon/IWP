using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCanvas : MonoBehaviour
{
    [Header("Upgradable Items")]
    [SerializeField] Image UpgradeItemsIcon;
    [SerializeField] TextMeshProUGUI UpgradeItemsType;
    [SerializeField] SlotPopup slotPopup;
    [SerializeField] ItemContentManager ItemContentManager;
    [SerializeField] EnhancementManager EnhancementManager;
    private Item itemREF;

    public Item GetItemREF()
    {
        return itemREF;
    }
    public SlotPopup SlotPopup
    {
        get { return slotPopup; }
    }

    private void OnDestroy()
    {
        ItemContentManager.UnSubscribeCurrentItem();
    }

    // Update is called once per frame
    public void OpenUpgradeItemCanvas(Item Item)
    {
        itemREF = Item;

        if (GetItemREF() == null)
            return;
        else if (GetItemREF() is not UpgradableItems)
            return;

        MainUI.GetInstance().OpenItemBag();
        transform.GetChild(0).gameObject.SetActive(true);
        EnhancementManager.Init();
        EnhancementManager.SetExpDisplay();
        slotPopup.Refresh();
        UpgradeItemsType.text = GetItemREF().GetItemSO().GetItemType() + " / " + GetItemREF().GetItemSO().ItemName;
        ItemContentManager.SetItemREF(GetItemREF(), GetItemREF().GetItemSO());
    }
}