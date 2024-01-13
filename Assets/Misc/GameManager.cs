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

        if (EM.GetCurrentWave() != 0 && EM.GetEnemiesCount() != 0)
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
        assetManager.OpenMessageNotification("Wave " + EM.GetCurrentWave() + " Incoming!");
        TotalEnemyInWave = CalculateHowManyToSpawn();
        EM.SetEnemiesCount(TotalEnemyInWave);

        SpawnEnemies(EM.GetCurrentWave(), 1f);
    }

    private void OnWaveComplete()
    {
        if (Teleporter != null)
            return;

        Teleporter = Instantiate(TeleporterPrefab, EnemyManager.GetRandomPointWithinTerrain(terrain), Quaternion.identity);
        MainUI.GetInstance().SpawnArrowIndicator(Teleporter);

        InventoryManager.GetInstance().AddCurrency(CurrencyType.COINS, Mathf.RoundToInt(50f + 50f * (EM.GetCurrentWave() - 1) * Random.Range(0.4f, 1f)));
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
        while (!EM.HasNotReachSpawnLimit() || EM.GetTotalEnemyAliveInList() >= (TotalEnemyInWave - EM.GetCurrentEnemyDefeated() - 1))
        {
            yield return null;
        }

        yield return new WaitForSeconds(sec);

        BaseEnemy enemy = EM.SpawnGroundUnitsWithinTerrain(charactersSO, terrain);
        enemy.OnDeadEvent += OnDead;
        SpawnEnemies(EM.GetCurrentWave(), sec);
    }

    private void OnDead(BaseEnemy e)
    {
        EM.EnemyDropRandomItem(e.GetCharactersSO(), e.GetPointOfContact());
        e.OnDeadEvent -= OnDead;
    }

    public int CalculateHowManyToSpawn()
    {
        int BaseRate = 3;
        return Mathf.RoundToInt(BaseRate + EM.GetCurrentWave() * (EM.GetCurrentWave() - 1) + BaseRate);
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
