using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UseItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UpgradeCanvasTransition upgradeCanvasTransition;
    [SerializeField] TextMeshProUGUI ButtonText;
    private ItemButton ItemButtonREF;

    public void SetItemREF(ItemButton ItemButtonREF)
    {
        this.ItemButtonREF = ItemButtonREF;

        switch(ItemButtonREF.GetItemREF())
        {
            case ConsumableItem c:
                ButtonText.text = "Use";
                break;
            case UpgradableItems u:
                ButtonText.text = "Details";
                break;
        }
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Use();
        DetailsPage();
    }

    private void DetailsPage()
    {
        if (ItemButtonREF == null)
            return;

        upgradeCanvasTransition.SetItemREF(ItemButtonREF.GetItemREF());
    }

    private void Use()
    {
        if (ItemButtonREF == null)
            return;

        ConsumableItem consumableItem = ItemButtonREF.GetItemREF() as ConsumableItem;
        if (consumableItem == null)
            return;

        consumableItem.Use(1);
    }
}
