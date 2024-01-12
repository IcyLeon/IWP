using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UseItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI ButtonText;
    private ItemButton ItemButtonREF;

    public void SetItemREF(ItemButton ItemButtonREF)
    {
        this.ItemButtonREF = ItemButtonREF;

        gameObject.SetActive(ItemButtonREF?.GetItemREF() is not ConsumableItemForbiddenInInventory && ItemButtonREF?.GetItemREF() is ConsumableItem);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Use();
    }

    private void Use()
    {
        if (ItemButtonREF == null)
            return;

        ConsumableItem consumableItem = ItemButtonREF.GetItemREF() as ConsumableItem;
        if (consumableItem == null)
            return;

        switch(consumableItem)
        {
            case Food food:
                if (food is RecoveryFood)
                {
                    AssetManager.GetInstance().SpawnFoodPanel(food);
                    return;
                }

                food.Use(1);
                break;
            default:
                consumableItem.Use(1);
                break;
        }
    }
}
