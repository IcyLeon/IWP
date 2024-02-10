using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BossManager : MonoBehaviour
{
    private static BossManager instance;
    private EnemyManager EM;
    private Coroutine SpawningCoroutine;
    [SerializeField] AudioSource audioMusicSource;
    [SerializeField] Terrain terrain;
    [SerializeField] GameObject TreePrefab;
    [SerializeField] GameObject TeleporterPrefab;
    private GameObject Teleporter;

    public void SpawnGroundUnitsWithinTerrain(CharactersSO charactersSO)
    {
        EM.SpawnGroundUnitsWithinTerrain(charactersSO, terrain);
    }

    private void OnWaveComplete()
    {
        if (Teleporter != null)
            return;

        Teleporter = Instantiate(TeleporterPrefab, EnemyManager.GetRandomPointWithinTerrain(terrain), Quaternion.identity);
        audioMusicSource.volume = 0f;
        AssetManager.CallSpawnArrow(Teleporter, Color.green);

        InventoryManager.GetInstance().AddCurrency(CurrencyType.CASH, Mathf.RoundToInt(180f + 180f * (EnemyManager.GetCurrentWave() - 1) * Random.Range(0.4f, 1f)));
    }

    private bool IsAllBossAreDead()
    {
        return EM.GetBossEnemyList().Count == 0;
    }

    public static BossManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (terrain == null)
        {
            GameObject terrainGO = GameObject.FindGameObjectWithTag("Terrain");
            if (terrainGO == null)
                return;

            terrain = terrainGO.GetComponent<Terrain>();
        }
        EM = EnemyManager.GetInstance();

        if (terrain)
            FriendlyKillerHandler.GetInstance().LoadKillersAroundTerrain(terrain);

        EM.SetCurrentWave(EnemyManager.GetCurrentWave() + 1);


    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        if (IsAllBossAreDead())
        {
            if (SpawningCoroutine == null)
                SpawningCoroutine = StartCoroutine(SpawningOfTree());
        }
    }


    private IEnumerator SpawningOfTree()
    {
        EM.DestroyAllEnemies();
        OnWaveComplete();

        yield return new WaitForSeconds(3f);

        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.GetPosition();
        Vector3 middlePosition = terrainPosition + terrainSize / 2f;
        float terrainHeight = terrain.SampleHeight(middlePosition);
        Vector3 spawnPosition = new Vector3(middlePosition.x, terrainHeight, middlePosition.z);

        GameObject tree = Instantiate(TreePrefab, spawnPosition, Quaternion.identity);

    }
}
