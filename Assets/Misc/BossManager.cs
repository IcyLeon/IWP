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
        MainUI.GetInstance().SpawnArrowIndicator(Teleporter);
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
        EM = EnemyManager.GetInstance();
        FriendlyKillerHandler.GetInstance().LoadKillersAroundTerrain(terrain);

        EM.SetCurrentWave(EM.GetCurrentWave() + 1);
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

        Bounds terrainBounds = terrain.terrainData.bounds;
        Vector3 terrainCenter = terrainBounds.center;
        Vector3 localTerrainCenter = terrain.transform.InverseTransformPoint(terrainCenter);
        float terrainHeight = terrain.SampleHeight(localTerrainCenter);
        Vector3 spawnPosition = new Vector3(terrainCenter.x, terrainHeight, terrainCenter.z);

        GameObject tree = Instantiate(TreePrefab, spawnPosition, Quaternion.identity);

    }
}
