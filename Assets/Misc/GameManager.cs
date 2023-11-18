using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class GameSpawnInfo
    {
        public CharactersSO[] IntroduceEnemySO;
        public int Wave;
    }

    [SerializeField] GameSpawnInfo[] GameSpawnInfoList;
    [SerializeField] GameSpawnInfo[] BossSpawnInfoList;
    [SerializeField] Terrain terrain;
    private List<BaseEnemy> CurrentEnemySpawnedList;
    private EnemyManager EM;
    private int MaxEnemyInScene = 25;
    private static GameManager instance;
    private int TotalEnemyInWave;
    private Coroutine WaveSpawnCoroutine;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EM = EnemyManager.GetInstance();
        CurrentEnemySpawnedList = new();
        TotalEnemyInWave = 0;
        EM.OnEnemyWaveChange += OnWaveComplete;
    }

    public void NextWave()
    {
        if (EM.GetCurrentEnemyDefeated() != TotalEnemyInWave && EM.GetCurrentWave() != 0)
            return;

        EM.SetCurrentWave(EM.GetCurrentWave() + 1);
        if (EM.GetCurrentWave() % 5 == 0)
            SpawnBoss();

        TotalEnemyInWave = CalculateHowManyToSpawn();
        EM.SetEnemiesCount(TotalEnemyInWave);
        WaveSpawn(EnemyType.ALBINO, 0);
    }

    private void OnWaveComplete()
    {

    }

    private void WaveSpawn(EnemyType enemyType, float delay)
    {
        if (WaveSpawnCoroutine != null)
        {
            StopCoroutine(WaveSpawnCoroutine);
        }

        WaveSpawnCoroutine = StartCoroutine(WaveSpawnDelay(enemyType, delay));
    }

    private IEnumerator WaveSpawnDelay(EnemyType enemyType, float sec)
    {
        while ((CurrentEnemySpawnedList.Count >= MaxEnemyInScene || CurrentEnemySpawnedList.Count >= TotalEnemyInWave - EM.GetCurrentEnemyDefeated()))
        {
            yield return null;
        }

        yield return new WaitForSeconds(sec);

        SpawnGroundUnitsWithinTerrain(enemyType);
        WaveSpawn(enemyType, sec);
    }

    private void SpawnGroundUnitsWithinTerrain(EnemyType e)
    {
        TerrainData terrainData = terrain.terrainData;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;
        float randomX = Random.Range(0f, terrainWidth);
        float randomZ = Random.Range(0f, terrainLength);
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0f, randomZ));
        Vector3 spawnPosition = new Vector3(randomX, terrainHeight, randomZ);

        // Sample the position on the NavMesh
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 500.0f, NavMesh.AllAreas))
        {
            BaseEnemy enemy = Instantiate(EM.GetEnemyInfo(e).EnemyPrefab, hit.position, Quaternion.identity).GetComponent<BaseEnemy>();
            CurrentEnemySpawnedList.Add(enemy);
        }
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
    }

    private void RemoveInactive()
    {
        for (int i = CurrentEnemySpawnedList.Count - 1; i > 0; i--)
        {
            if (CurrentEnemySpawnedList[i] == null)
                CurrentEnemySpawnedList.RemoveAt(i);
        }
    }

    public static GameManager GetInstance()
    {
        return instance;
    }
}
