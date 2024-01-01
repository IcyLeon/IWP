using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyData
{
    public bool ShieldStatus = false;

    public float CurrentStunElasped = 0f;
    public float stunDuration = 8f;

    public float TargetColliderHeight = 3f;
    public float ShootLaserDelay = 0.5f;
}
