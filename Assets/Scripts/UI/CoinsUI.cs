using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    private InventoryManager inventoryManager;
    [SerializeField] TextMeshProUGUI CoinsTxt;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = InventoryManager.GetInstance();
        inventoryManager.GetPlayerStats().OnCoinsChanged += OnValueChanged;
        OnValueChanged();
    }

    private void OnValueChanged()
    {
        CoinsTxt.text = inventoryManager.GetCoins().ToString();
    }
}
