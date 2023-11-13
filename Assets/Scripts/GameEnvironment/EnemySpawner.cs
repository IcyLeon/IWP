using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    ALBINO,
    SPITTER
}

public class EnemySpawner : MonoBehaviour
{
    public class EnemyInfo
    {
        public EnemyType enemyType;
        public GameObject EnemyPrefab;
    }

    [SerializeField] EnemyInfo[] EnemyInfoList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
