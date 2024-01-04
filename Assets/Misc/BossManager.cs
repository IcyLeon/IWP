using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static BossManager instance;
    private EnemyManager EM;
    [SerializeField] Terrain terrain;

    public void SpawnGroundUnitsWithinTerrain(EnemyType e)
    {
        EM.SpawnGroundUnitsWithinTerrain(e, terrain);
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
