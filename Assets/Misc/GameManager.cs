using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class GameSpawnInfo
    {
        public ScriptableObject ScriptableObjectSO;
        public int Wave;
    }

    [SerializeField] GameObject TeleporterPrefab;
    [SerializeField] GameSpawnInfo[] GameSpawnInfoList;
    [SerializeField] Transform WaypointSpawnPointListGO;
    [SerializeField] Terrain terrain;
    private EnemyManager EM;
    private static GameManager instance;
    private int TotalEnemyInWave;
    private Coroutine WaveSpawnCoroutine;
    private AssetManager assetManager;
    private bool isCompleted;
    private GameObject Teleporter;
    [SerializeField] CharactersSO tempcharactersSO;

    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.OnSceneChanged += OnSceneChanged;
        EnemyManager.OnEnemyKilled += OnEnemyKilled;
        instance = this;
    }

    private void Start()
    {
        EM = EnemyManager.GetInstance();
        assetManager = AssetManager.GetInstance();
        isCompleted = false;
        TotalEnemyInWave = 0;
        FriendlyKillerHandler.GetInstance().LoadKillersAroundTerrain(terrain);
    }

    private void SpawnEnemies(int Wave, float sec)
    {
        int random = Random.Range(0, GameSpawnInfoList.Length);

        if (Wave < GameSpawnInfoList[random].Wave)
        {
            SpawnEnemies(Wave, sec);
            return;
        }

        CharactersSO CharactersSO = GameSpawnInfoList[random].ScriptableObjectSO as CharactersSO;
        WaveSpawn(CharactersSO, sec);
        SpawnArrowNearestEnemies();
    }

    private void OnSceneChanged(SceneEnum s)
    {
        StartCoroutine(SetRandomLocation());
    }
    private IEnumerator SetRandomLocation()
    {
        yield return null;
        CharacterManager.GetInstance().GetPlayerManager().transform.position = GetRandomSpawnPoint();
    }
    private Vector3 GetRandomSpawnPoint()
    {
        Transform[] WaypointList = WaypointSpawnPointListGO.GetComponentsInChildren<Transform>(true)
            .Where(childTransform => childTransform != WaypointSpawnPointListGO.transform)
            .ToArray();

        int random = Random.Range(0, WaypointList.Length);
        Vector3 point = WaypointList[random].position;
        float terrainHeight = terrain.SampleHeight(new Vector3(point.x, 0f, point.z));
        Vector3 actposition = new Vector3(point.x, terrainHeight + terrain.GetPosition().y, point.z);
        return actposition;
    }

    private void OnEnemyKilled(BaseEnemy enemy)
    {
        EM.SetCurrentEnemyDefeated(EM.GetCurrentEnemyDefeated() + 1);
    }

    private void OnDestroy()
    {
        EM.ResetCounter();
        SceneManager.OnSceneChanged -= OnSceneChanged;
        EnemyManager.OnEnemyKilled -= OnEnemyKilled;
    }

    public void UpdateWave()
    {
        if (Time.timeScale == 0)
            return;

        if (EnemyManager.GetCurrentWave() != 0 && EM.GetEnemiesCount() != 0)
        {
            if (EM.GetCurrentEnemyDefeated() < TotalEnemyInWave)
            {
                return;
            }
            else
            {
                if (!isCompleted && EM.GetCurrentEnemyDefeated() >= EM.GetEnemiesCount())
                {
                    assetManager.OpenMessageNotification("Wave " + EnemyManager.GetCurrentWave() + " Complete");
                    OnWaveComplete();
                    isCompleted = true;
                    return;
                }
            }
        }

        if (isCompleted)
            return;

        EM.SetCurrentWave(EnemyManager.GetCurrentWave() + 1);
        assetManager.OpenMessageNotification("Wave " + EnemyManager.GetCurrentWave() + " Incoming!");
        TotalEnemyInWave = CalculateHowManyToSpawn();
        EM.SetEnemiesCount(TotalEnemyInWave);

        SpawnEnemies(EnemyManager.GetCurrentWave(), 4f);
    }

    private void OnWaveComplete()
    {
        if (Teleporter != null)
            return;

        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.GetPosition();
        Vector3 middlePosition = terrainPosition + terrainSize / 2f;
        float terrainHeight = terrain.SampleHeight(middlePosition);
        Vector3 spawnPosition = new Vector3(middlePosition.x, terrainHeight + terrainPosition.y,  middlePosition.z);

        Teleporter = Instantiate(TeleporterPrefab, spawnPosition, Quaternion.identity);
        AssetManager.CallSpawnArrow(Teleporter, Color.green);

        InventoryManager.GetInstance().AddCurrency(CurrencyType.CASH, Mathf.RoundToInt(50f + 50f * (EnemyManager.GetCurrentWave() - 1) * Random.Range(0.4f, 1f)));
    }

    private void WaveSpawn(CharactersSO charactersSO, float delay)
    {
        if (WaveSpawnCoroutine != null)
        {
            StopCoroutine(WaveSpawnCoroutine);
        }

        WaveSpawnCoroutine = StartCoroutine(WaveSpawnDelay(charactersSO, delay));
    }

    private IEnumerator WaveSpawnDelay(CharactersSO charactersSO, float sec)
    {
        while (!EM.HasNotReachSpawnLimit() || EM.GetTotalEnemyAliveInList() >= (EM.GetEnemiesCount() - EM.GetCurrentEnemyDefeated() - 1))
        {
            yield return null;
        }

        yield return new WaitForSeconds(sec);

        BaseEnemy enemy = EM.SpawnGroundUnitsWithinTerrain(charactersSO, terrain);
        enemy.OnDeadEvent += OnDead;

        float actualTimer = Random.Range(sec - 2.5f, sec + 2.5f);
        actualTimer = Mathf.Max(1f, actualTimer);

        SpawnEnemies(EnemyManager.GetCurrentWave(), actualTimer);
    }

    private void OnDead(BaseEnemy e)
    {
        SpawnArrowNearestEnemies();
        EM.EnemyDropRandomItem(e.GetCharactersSO(), e.GetPointOfContact());
        e.OnDeadEvent -= OnDead;
    }

    private void SpawnArrowNearestEnemies()
    {
        int percentageDiff = EM.GetEnemiesCount() - EM.GetCurrentEnemyDefeated();
        if (percentageDiff <= 4f)
        {
            for (int i = 0; i < EM.GetCurrentEnemySpawnedList().Count; i++) {
                BaseEnemy enemy = EM.GetCurrentEnemySpawnedList()[i];
                if (enemy != null)
                {
                    if (!enemy.IsDead())
                    {
                        AssetManager.CallSpawnArrow(enemy.gameObject, Color.red);
                    }
                }
            }
        }
    }

    public int CalculateHowManyToSpawn()
    {
        int BaseRate = 8;
        int total = Mathf.RoundToInt(BaseRate + (EnemyManager.GetCurrentWave() - 1) * BaseRate);
        total = Mathf.Clamp(total, 0, 80);
        return total;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWave();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }
}
