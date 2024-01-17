using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestHandler : MonoBehaviour
{
    [SerializeField] GameObject[] ChestPrefabs;
    private static ChestHandler instance;
    private int MaxChestSpawn = 8;
    public static ChestHandler GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.OnSceneChanged += OnSceneChanged;
    }

    // Start is called before the first frame update
    void OnSceneChanged(SceneEnum s)
    {
        switch(s)
        {
            case SceneEnum.GAME:
                int randomAmt = Random.Range(0, MaxChestSpawn + 1);
                for (int i = 0; i < randomAmt; i++)
                {
                    SpawnChestWithinTerrain();
                }
                break;
        }
    }

    public void SpawnChestWithinTerrain()
    {
        GameObject terrainGO = GameObject.FindGameObjectWithTag("Terrain");
        if (terrainGO == null)
            return;

        Terrain terrain = terrainGO.GetComponent<Terrain>();
        if (terrain == null)
            return;

        Vector3 spawnPosition = EnemyManager.GetRandomPointWithinTerrain(terrain);
        int randomIndex = Random.Range(0, ChestPrefabs.Length);

        Instantiate(ChestPrefabs[randomIndex], spawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        SceneManager.OnSceneChanged -= OnSceneChanged;
    }
}
