using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyType : MonoBehaviour
{
    private EnemyManager EM;
    private BaseEnemy baseEnemy;
    // Start is called before the first frame update
    void Start()
    {
        EM = EnemyManager.GetInstance();
        baseEnemy = GetComponent<BaseEnemy>();
        SubscribeToBoss(baseEnemy);
    }

    // Update is called once per frame
    void SubscribeToBoss(BaseEnemy BaseEnemy)
    {
        EM.AddBossToList(BaseEnemy);
    }
}
