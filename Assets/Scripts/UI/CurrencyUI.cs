using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CurrencyType
{
    COINS,
    CASH
}

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] Image CurrencyImage;
    [SerializeField] CurrencyType currencyType;
    [SerializeField] TextMeshProUGUI CurrencyValueTxt;
    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = InventoryManager.GetInstance();
        PlayerStats.OnCoinsChanged += OnCoinsChanged;
        PlayerStats.OnCashChanged += OnCashsChanged;
        OnCoinsChanged();
        OnCashsChanged();
        Init();
    }

    void Init()
    {
        CurrencyImage.sprite = AssetManager.GetInstance().GetCurrencySprite(currencyType);
    }

    // Update is called once per frame
    private void OnCoinsChanged()
    {
        if (currencyType == CurrencyType.COINS)
        {
            CurrencyValueTxt.text = inventoryManager.GetCurrency(CurrencyType.COINS).ToString();
        }
    }

    private void OnCashsChanged()
    {
        if (currencyType == CurrencyType.CASH)
        {
            CurrencyValueTxt.text = inventoryManager.GetCurrency(CurrencyType.CASH).ToString();
        }
    }

    private void OnDestroy()
    {
        PlayerStats.OnCoinsChanged -= OnCoinsChanged;
        PlayerStats.OnCashChanged -= OnCashsChanged;
    }
}
