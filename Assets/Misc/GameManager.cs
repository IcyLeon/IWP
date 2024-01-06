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
    private EnemyManager EM;
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
        EM.OnEnemyKilled += OnEnemyKilled;
        isCompleted = false;
        TotalEnemyInWave = 0;
        LoadKillersAroundTerrain();
    }

    private void LoadKillersAroundTerrain()
    {
        FriendlyKillerHandler friendlyKillerHandler = FriendlyKillerHandler.GetInstance();
        foreach(FriendlyKillerData f in friendlyKillerHandler.GetFriendlyKillerDataList())
        {
            Vector3 pos = EnemyManager.GetRandomPointWithinTerrain(terrain);
            FriendlyKillers friendlyKillers = Instantiate(friendlyKillerHandler.GetFriendlyKillerInfo(f.GetFriendlyKillerSO()).FriendlyKillerPrefab, pos, Quaternion.identity).GetComponent<FriendlyKillers>(); ;
            if (friendlyKillers != null)
                friendlyKillers.SetKillerData(f);
        }
    }

    private void OnEnemyKilled(BaseEnemy enemy)
    {
        EM.SetCurrentEnemyDefeated(EM.GetCurrentEnemyDefeated() + 1);
    }

    private void OnDestroy()
    {
        EM.ResetCounter();
        EM.OnEnemyKilled -= OnEnemyKilled;
    }

    public void UpdateWave()
    {
        if (Time.timeScale == 0)
            return;

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

        Teleporter = Instantiate(TeleporterPrefab, EnemyManager.GetRandomPointWithinTerrain(terrain), Quaternion.identity);
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

    private IEnumerator WaveSpawnDelay(EnemyType enemyType, float sec)
    {
        while (EM.HasReachSpawnLimit() || EM.GetTotalEnemyAliveInList() >= (TotalEnemyInWave - EM.GetCurrentEnemyDefeated() - 1))
        {
            yield return null;
        }

        yield return new WaitForSeconds(sec);

        EM.SpawnGroundUnitsWithinTerrain(enemyType, terrain);
        WaveSpawn(enemyType, sec);
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
        UpdateWave();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }
}
