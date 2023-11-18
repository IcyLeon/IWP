using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EnemyType
{
    ALBINO,
    SPITTER
}

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    private int CurrentWave;
    private int CurrentEnemyDefeated;
    private int TotalEnemies;
    public event Action OnEnemyDefeatedChange;
    public event Action OnEnemyWaveChange;

    [System.Serializable]
    public class EnemyInfo
    {
        public EnemyType enemyType;
        public GameObject EnemyPrefab;
    }

    [SerializeField] EnemyInfo[] EnemyInfoList;

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

    public void ResetCounter()
    {
        SetCurrentEnemyDefeated(0);
    }
}
