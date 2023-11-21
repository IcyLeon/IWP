using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class GameSpawnInfo
    {
        public CharactersSO[] IntroduceEnemySO;
        public int Wave;
    }
    [SerializeField] GameObject TeleporterPrefab;
    [SerializeField] GameSpawnInfo[] GameSpawnInfoList;
    [SerializeField] GameSpawnInfo[] BossSpawnInfoList;
    [SerializeField] Terrain terrain;
    private List<BaseEnemy> CurrentEnemySpawnedList;
    private EnemyManager EM;
    private int MaxEnemyInScene = 25;
    private static GameManager instance;
    private int TotalEnemyInWave;
    private Coroutine WaveSpawnCoroutine;
    private AssetManager assetManager;
    private bool isCompleted;
    private GameObject Teleporter;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EM = EnemyManager.GetInstance();
        assetManager = AssetManager.GetInstance();
        CurrentEnemySpawnedList = new();
        isCompleted = false;
        TotalEnemyInWave = 0;
    }

    private void OnDestroy()
    {
        EM.ResetCounter();
    }

    public void NextWave()
    {
        if (EM.GetCurrentWave() != 0)
        {
            if (EM.GetCurrentEnemyDefeated() < TotalEnemyInWave)
            {
                return;
            }
            else
            {
                if (!isCompleted && EM.GetCurrentEnemyDefeated() >= EM.GetEnemiesCount())
                {
                    assetManager.OpenMessageNotification("Wave " + EM.GetCurrentWave() + " Complete");
                    OnWaveComplete();
                    isCompleted = true;
                    return;
                }
            }
        }

        if (isCompleted)
            return;

        EM.SetCurrentWave(EM.GetCurrentWave() + 1);
        if (EM.GetCurrentWave() % 5 == 0)
            SpawnBoss();

        assetManager.OpenMessageNotification("Wave " + EM.GetCurrentWave() + " Incoming!");
        TotalEnemyInWave = CalculateHowManyToSpawn();
        EM.SetEnemiesCount(TotalEnemyInWave);
        WaveSpawn(EnemyType.ALBINO, 0);
    }

    private void OnWaveComplete()
    {
        if (Teleporter != null)
            return;

        Teleporter = Instantiate(TeleporterPrefab, SpawnRandomlyWithinTerrain(), Quaternion.identity);
        MainUI.GetInstance().SpawnArrowIndicator(Teleporter);
    }

    private void WaveSpawn(EnemyType enemyType, float delay)
    {
        if (WaveSpawnCoroutine != null)
        {
            StopCoroutine(WaveSpawnCoroutine);
        }

        WaveSpawnCoroutine = StartCoroutine(WaveSpawnDelay(enemyType, delay));
    }

    private int GetTotalEnemyAlive()
    {
        int Count = 0;
        for(int i = 0; i < CurrentEnemySpawnedList.Count; i++)
        {
            BaseEnemy enemy = CurrentEnemySpawnedList[i];
            if (enemy != null)
            {
                if (!enemy.IsDead())
                    Count++;
            }
        }
        return Count;
    }

    private IEnumerator WaveSpawnDelay(EnemyType enemyType, float sec)
    {
        while (GetTotalEnemyAlive() >= MaxEnemyInScene || GetTotalEnemyAlive() >= (TotalEnemyInWave - EM.GetCurrentEnemyDefeated() - 1))
        {
            yield return null;
        }

        yield return new WaitForSeconds(sec);

        SpawnGroundUnitsWithinTerrain(enemyType);
        WaveSpawn(enemyType, sec);
    }

    private void SpawnGroundUnitsWithinTerrain(EnemyType e)
    {
        Vector3 spawnPosition = SpawnRandomlyWithinTerrain();

        BaseEnemy enemy = Instantiate(EM.GetEnemyInfo(e).EnemyPrefab, spawnPosition, Quaternion.identity).GetComponent<BaseEnemy>();
        CurrentEnemySpawnedList.Add(enemy);
    }

    private Vector3 SpawnRandomlyWithinTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;
        float randomX = Random.Range(0f, terrainWidth);
        float randomZ = Random.Range(0f, terrainLength);
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0f, randomZ));
        Vector3 spawnPosition = new Vector3(randomX, terrainHeight, randomZ);

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 200.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return default(Vector3);
    }

    private void SpawnBoss()
    {

    }

    public int CalculateHowManyToSpawn()
    {
        int BaseRate = 1;
        return Mathf.RoundToInt(BaseRate + EM.GetCurrentWave() * (EM.GetCurrentWave() - 1) + BaseRate);
    }

    // Update is called once per frame
    void Update()
    {
        RemoveInactive();
        NextWave();
        Debug.Log(isCompleted);
    }

    private void RemoveInactive()
    {
        for (int i = CurrentEnemySpawnedList.Count - 1; i > 0; i--)
        {
            if (CurrentEnemySpawnedList[i] == null)
            {
                CurrentEnemySpawnedList.RemoveAt(i);
            }
        }
    }

    public static GameManager GetInstance()
    {
        return instance;
    }
}
