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

        gameObject.SetActive(true);
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

        consumableItem.Use(1);
    }
}
