using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UseItem : MonoBehaviour, IPointerClickHandler
{
    private Item ItemREF;

    public void SetItemREF(Item ItemREF)
    {
        this.ItemREF = ItemREF;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Use();
    }

    private void Use()
    {
        ConsumableItem consumableItem = ItemREF as ConsumableItem;
        if (consumableItem == null)
            return;

        consumableItem.Use(1);
    }
}
