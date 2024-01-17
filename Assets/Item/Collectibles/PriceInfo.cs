using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PriceInfo : MonoBehaviour
{
    [SerializeField] Image CurrencyImage;
    [SerializeField] TextMeshProUGUI CurrencyValueTxt;

    // Update is called once per frame
    public void UpdateContent(CurrencyType c, int amt)
    {
        if (CurrencyImage)
            CurrencyImage.sprite = AssetManager.GetInstance().GetCurrencySprite(c);
        if (CurrencyValueTxt)
            CurrencyValueTxt.text = amt.ToString();
    }
}
