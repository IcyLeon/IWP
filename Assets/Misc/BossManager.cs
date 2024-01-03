using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static BossManager instance;
    private EnemyManager EM;
    [SerializeField] Terrain terrain;

    public BaseEnemy SpawnGroundUnitsWithinTerrain(EnemyType e)
    {
        BaseEnemy enemy = EM.SpawnGroundUnitsWithinTerrain(e, terrain);
        return enemy;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
