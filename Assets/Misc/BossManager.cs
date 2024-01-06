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
    private bool isCompleted;
    [SerializeField] Terrain terrain;
    [SerializeField] GameObject TreePrefab;

    public void SpawnGroundUnitsWithinTerrain(EnemyType e)
    {
        EM.SpawnGroundUnitsWithinTerrain(e, terrain);
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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        if (SpawningCoroutine == null && IsAllBossAreDead())
        {
            SpawningCoroutine = StartCoroutine(SpawningOfTree());
        }
    }

    private IEnumerator SpawningOfTree()
    {
        yield return new WaitForSeconds(3f);

        Bounds terrainBounds = terrain.terrainData.bounds;
        Vector3 terrainCenter = terrainBounds.center;

        if (NavMesh.SamplePosition(terrainCenter, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            Vector3 middlePosition = hit.position;
            GameObject tree = Instantiate(TreePrefab, middlePosition, Quaternion.identity);
        }

    }
}
