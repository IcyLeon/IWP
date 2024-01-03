using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyData
{
    public bool ShieldStatus = false;
    public float ShieldCooldown = 10f;
    public float ShieldCurrentElasped = 0f;

    public float AttackInterval = 4f;
    public float AttackCurrentElasped = 0f;
    public int CurrentAttackIndex = 1;

    public float CurrentStunElasped = 0f;
    public float stunDuration = 6f;

    public int SpawnEnemiesAmount = 3;

    public float timeToReachTargetRotation { get; } = 0.14f;
    public float dampedTargetRotationCurrentVelocity;
    public float dampedTargetRotationPassedTime;
    public Quaternion CurrentTargetRotation, Target_Rotation;

    public MageEnemyData()
    {
        ShieldCurrentElasped = ShieldCooldown;
    }
}
