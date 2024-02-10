using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    private Camera Cam;
    [Header("Environment")]
    [SerializeField] GameObject[] PillarSectionPrefab;

    [Header("Bridge")]
    [SerializeField] GameObject BridgePrefab;
    [SerializeField] GameObject Bridge2Prefab;
    [SerializeField] Transform Spawner, SpawnerPillars;
    [SerializeField] float SpawnerPillarsSectionOffsetLength;
    [SerializeField] float SpawnerOffsetLength;
    [SerializeField] float SpawnTimer;
    [SerializeField] Transform BridgeParent, PillarsParent;
    private float LengthToDisappear = 3f;
    List<GameObject> Bridges = new List<GameObject>();
    List<GameObject> Pillars = new List<GameObject>();
    private Coroutine SpawnBridge;
    private Coroutine SpawnPillars;
    private int regularBridgeSpawned = 3;

    public float GetSpawnerOffsetLength()
    {
        return SpawnerOffsetLength;
    }
    public Transform GetSpawnerTransform()
    {
        return Spawner;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach(var bridge in BridgeParent.GetComponentsInChildren<Transform>())
        {
            if (bridge == BridgeParent)
                continue;

            Bridges.Add(bridge.gameObject);
        }

        Cam = Camera.main;
    }

    public float GetSpawnTimer()
    {
        return SpawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnBridge == null)
        {
            GetSpawnerTransform().position += Vector3.forward * GetSpawnerOffsetLength();
            SpawnBridge = StartCoroutine(SpawnBridgeCoroutine(GetSpawnerTransform().position));
        }
        if (SpawnPillars == null)
        {
            SpawnPillars = StartCoroutine(SpawnPillarsSectionCoroutine(SpawnerPillars.position));
            SpawnerPillars.position += Vector3.forward * SpawnerPillarsSectionOffsetLength;
        }

        UpdateBridgeLifeTime();
    }

    private IEnumerator SpawnPillarsSectionCoroutine(Vector3 pos)
    {
        CreatePillarSections(pos);

        yield return new WaitUntil(() => (Cam.transform.position - SpawnerPillars.position).magnitude <= SpawnerPillarsSectionOffsetLength * 2f);
        SpawnPillars = null;
    }

    private void CreatePillarSections(Vector3 pos)
    {
        GameObject go = Instantiate(PillarSectionPrefab[0], pos, Quaternion.identity);
        foreach (var pillar in go.GetComponentsInChildren<Transform>())
        {
            if (pillar == go)
                continue;

            pillar.SetParent(PillarsParent);
            Pillars.Add(pillar.gameObject);
        }
        Destroy(go.gameObject);
    }

    private IEnumerator SpawnBridgeCoroutine(Vector3 pos)
    {
        if (regularBridgeSpawned < 3)
        {
            Bridges.Add(CreateBridge1(pos));
            regularBridgeSpawned++;
        }
        else
        {
            Bridges.Add(CreateBridge2(pos));
            regularBridgeSpawned = 0;
        }
        yield return new WaitForSeconds(SpawnTimer);
        SpawnBridge = null;
    }

    private GameObject CreateBridge1(Vector3 position)
    {
        GameObject go = Instantiate(BridgePrefab, position, BridgePrefab.transform.rotation);
        go.transform.SetParent(BridgeParent);
        return go;
    }
    private GameObject CreateBridge2(Vector3 position)
    {
        GameObject go = Instantiate(Bridge2Prefab, position, BridgePrefab.transform.rotation);
        go.transform.SetParent(BridgeParent);
        return go;
    }

    private void UpdateBridgeLifeTime()
    {
        for (int i = Bridges.Count - 1; i >= 0; i--)
        {
            GameObject go = Bridges[i];

            if (go == null)
            {
                Bridges.Remove(go);
            }
            else {
                Vector3 dir = go.transform.position - Cam.transform.position;

                if (dir.magnitude > LengthToDisappear * LengthToDisappear && Vector3.Dot(Cam.transform.forward, dir.normalized) < 0)
                {
                    Destroy(go.gameObject);
                }
            }
        }

        for (int i = Pillars.Count - 1; i >= 0; i--)
        {
            GameObject go = Pillars[i];

            if (go == null)
            {
                Pillars.Remove(go);
            }
            else
            {
                Vector3 dir = go.transform.position - Cam.transform.position;

                if (dir.magnitude > LengthToDisappear * LengthToDisappear && Vector3.Dot(Cam.transform.forward, dir.normalized) < 0)
                {
                    Destroy(go.gameObject);
                }
            }
        }
    }
}
