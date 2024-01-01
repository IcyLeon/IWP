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
    private List<BaseEnemy> BossEnemyList = new();
    public delegate void onEnemyChanged(BaseEnemy enemy);
    public onEnemyChanged OnEnemyKilled;
    public onEnemyChanged OnBossEnemyAdd;
    public onEnemyChanged OnBossEnemyRemove;

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

    public List<BaseEnemy> GetBossEnemyList()
    {
        return BossEnemyList;
    }
    public void AddBossToList(BaseEnemy e)
    {
        if (e != null)
        {
            BossEnemyList.Add(e);
            OnBossEnemyAdd?.Invoke(e);
        }
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

    private void Update()
    {
        UpdateRemoveBossEnemyList();
    }

    private void UpdateRemoveBossEnemyList()
    {
        for (int i = BossEnemyList.Count - 1; i > 0; i--)
        {
            if (BossEnemyList[i] == null)
            {
                BossEnemyList.RemoveAt(i);
            }
            else
            {
                if (BossEnemyList[i].IsDead())
                {
                    OnBossEnemyRemove?.Invoke(BossEnemyList[i]);
                    BossEnemyList.RemoveAt(i);
                }
            }
        }
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
