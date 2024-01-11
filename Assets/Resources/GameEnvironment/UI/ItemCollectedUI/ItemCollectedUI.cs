using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollectedUI : MonoBehaviour
{
    [SerializeField] Image ItemImage;
    [SerializeField] TextMeshProUGUI ItemTitleTxt;
    [SerializeField] TextMeshProUGUI ItemSummaryDescTxt;

    // Start is called before the first frame update
    public void Init(ItemTemplate itemTemplate)
    {
        if (itemTemplate == null)
            return;

        ItemImage.sprite = itemTemplate.ItemSprite;
        ItemTitleTxt.text = itemTemplate.ItemName;
        ItemSummaryDescTxt.text = itemTemplate.SummaryItemDesc;
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
