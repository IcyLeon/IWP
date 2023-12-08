using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public enum EnemyType
{
    ALBINO,
    SPITTER
}

public class EnemyManager : MonoBehaviour
{
    public delegate void onEnemyKilled(BaseEnemy enemy);
    public onEnemyKilled OnEnemyKilled;
    private static EnemyManager instance;
    private int CurrentWave;
    private int CurrentEnemyDefeated;
    private int TotalEnemies;
    private int TotalEnemiesDefeated;
    public event Action OnEnemyDefeatedChange;
    public event Action OnEnemyWaveChange;

    [System.Serializable]
    public class EnemyInfo
    {
        public EnemyType enemyType;
        public GameObject EnemyPrefab;
    }

    [SerializeField] EnemyInfo[] EnemyInfoList;

    public void CallOnEnemyKilled(BaseEnemy enemy)
    {
        TotalEnemiesDefeated++;
        InventoryManager.GetInstance().AddCurrency(CurrencyType.CASH, Mathf.RoundToInt(enemy.GetDropValue()));
        OnEnemyKilled?.Invoke(enemy);
    }

    public EnemyInfo GetEnemyInfo(EnemyType type)
    {
        for (int i = 0; i < EnemyInfoList.Length; i++)
        {
            if (type == EnemyInfoList[i].enemyType)
                return EnemyInfoList[i];
        }
        return null;
    }

    public static EnemyManager GetInstance()
    {
        return instance;
    }

    public void SetEnemiesCount(int value)
    {
        TotalEnemies = value;
    }

    public int GetEnemiesCount()
    {
        return TotalEnemies;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetTotalEnemiesDefeated();
        SetCurrentWave(0);
    }

    public void SetCurrentWave(int value)
    {
        CurrentWave = value;
        ResetCounter();
        OnEnemyWaveChange?.Invoke();
    }

    public int GetCurrentWave()
    {
        return CurrentWave;
    }

    public int GetCurrentEnemyDefeated()
    {
        return CurrentEnemyDefeated;
    }

    public void SetCurrentEnemyDefeated(int value)
    {
        CurrentEnemyDefeated = value;
        OnEnemyDefeatedChange?.Invoke();
    }

    public void ResetTotalEnemiesDefeated()
    {
        TotalEnemiesDefeated = 0;
    }

    public int GetTotalEnemyDefeated()
    {
        return TotalEnemiesDefeated;
    }


    public void ResetCounter()
    {
        SetCurrentEnemyDefeated(0);
    }

    public void ResetEverything()
    {
        ResetTotalEnemiesDefeated();
        SetEnemiesCount(0);
        SetCurrentWave(0);
    }
}
