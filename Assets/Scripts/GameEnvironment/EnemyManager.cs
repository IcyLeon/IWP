using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine.AI;

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
    public Action OnEnemyDefeatedChange;
    public Action OnEnemyWaveChange;

    [System.Serializable]
    public class EnemyInfo
    {
        public EnemyType enemyType;
        public GameObject EnemyPrefab;
    }

    [SerializeField] EnemyInfo[] EnemyInfoList;

    public static Vector3 GetRandomPointWithinTerrain(Terrain terrain)
    {
        // Use terrain bounds for more accurate random points
        Bounds terrainBounds = terrain.GetComponent<Collider>().bounds;

        float randomX = Random.Range(terrainBounds.min.x, terrainBounds.max.x);
        float randomZ = Random.Range(terrainBounds.min.z, terrainBounds.max.z);

        // Ensure the Y position is correctly sampled from the terrain
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0f, randomZ));

        // Adjust the spawn position to be above the terrain surface
        Vector3 spawnPosition = new Vector3(randomX, terrainHeight, randomZ);

        // Use NavMesh.SamplePosition to get a valid point on the NavMesh
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 200.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // If no valid point is found, return Vector3.zero or some other default value
        return Vector3.zero;
    }

    public BaseEnemy SpawnGroundUnitsWithinTerrain(EnemyType e, Terrain terrain)
    {
        Vector3 spawnPosition = GetRandomPointWithinTerrain(terrain);

        BaseEnemy enemy = Instantiate(GetEnemyInfo(e).EnemyPrefab, spawnPosition, Quaternion.identity).GetComponent<BaseEnemy>();
        return enemy;
    }

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
            OnBossEnemyAdd?.Invoke(e);
            BossEnemyList.Add(e);
        }
    }

    public EnemyInfo[] GetEnemyInfosList()
    {
        return EnemyInfoList;
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
        for (int i = BossEnemyList.Count - 1; i >= 0; i--)
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
