using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI StatsNameTxt;
    [SerializeField] TextMeshProUGUI StatsValueTxt;
    [SerializeField] StatsInfoSO StatsInfoSO;
    private EnemyManager EnemyManager;
    private InventoryManager InventoryManager;

    private void Start()
    {
        EnemyManager = EnemyManager.GetInstance();
        InventoryManager = InventoryManager.GetInstance();
        EnemyManager.OnEnemyKilled += UpdateEnemyKilled;
        InventoryManager.GetPlayerStats().OnCoinsChanged += UpdateCoins;
        InventoryManager.GetPlayerStats().OnCashChanged += UpdateCash;
        UpdateContent();
    }

    private void UpdateContent()
    {
        if (StatsNameTxt != null)
        {
            StatsNameTxt.text = StatsInfoSO.StatsName;
            StatsValueTxt.text = GetStatsValue();
        }
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemyKilled -= UpdateEnemyKilled;
        InventoryManager.GetPlayerStats().OnCoinsChanged -= UpdateCoins;
        InventoryManager.GetPlayerStats().OnCashChanged -= UpdateCash;
    }

    private void UpdateCoins()
    {
        if (StatsInfoSO.statsInfoType != StatsInfoType.TOTAL_COINS)
            return;

        UpdateContent();
    }

    private void UpdateCash()
    {
        if (StatsInfoSO.statsInfoType != StatsInfoType.TOTAL_CASH)
            return;

        UpdateContent();
    }

    private void UpdateEnemyKilled(BaseEnemy enemy)
    {
        if (StatsInfoSO.statsInfoType != StatsInfoType.ENEMY_KILL)
            return;

        UpdateContent();
    }

    // Update is called once per frame
    private string GetStatsValue()
    {
        switch(StatsInfoSO.statsInfoType)
        {
            case StatsInfoType.ENEMY_KILL:
                return EnemyManager.GetTotalEnemyDefeated().ToString();
            case StatsInfoType.TOTAL_CASH:
                return InventoryManager.GetCurrency(CurrencyType.CASH).ToString();
            case StatsInfoType.TOTAL_COINS:
                return InventoryManager.GetCurrency(CurrencyType.COINS).ToString();
        }
        return null;
    }
}
