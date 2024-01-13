using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyData
{
    public enum MagePhase
    {
        Phase_1,
        Phase_2,
    }

    public MagePhase Phase;
    public bool ShieldStatus = false;
    public float ShieldCooldown = 10f;
    public float ShieldCurrentElasped = 0f;

    public int NoOfCrystalsOrb = 2;

    private float OriginalAttackInterval = 1.5f;
    public float AttackInterval;
    public float AttackCurrentElasped = 0f;
    public float CallingEnemiesInterval = 10f;
    public float CallingEnemiesElasped = 0f;
    public int CurrentAttackIndex = 1;

    public float CurrentStunElasped = 0f;
    public float stunDuration = 6f;

    public int SpawnEnemiesAmount = 2;

    public float CrystalsCoreStayDuration = 18f;
    public float CrystalsCoreStayElasped = 0f;
    public float CrystalsCoreSpawnDuration;
    private float OriginalCrystalsCoreSpawnDuration = 25f;
    public float CrystalsCoreSpawnElasped = 0f;

    public float TakeOffCooldown = 15f;
    public float TakeOffElapsed = 0f;

    public float FireBallSpawnDuration = 1f;
    public float FireBallSpawnElapsed = 0f;

    public float timeToReachTargetRotation { get; } = 0.14f;
    public float dampedTargetRotationCurrentVelocity;
    public float dampedTargetRotationPassedTime;
    public Quaternion CurrentTargetRotation, Target_Rotation;


    public int NoOfFireball = 6;

    public MageEnemyData()
    {
        ShieldCurrentElasped = ShieldCooldown;
        TakeOffElapsed = TakeOffCooldown;
        FireBallSpawnElapsed = FireBallSpawnDuration;
        CrystalsCoreSpawnDuration = GetRandomSpawnCrystalsInterval();
        CrystalsCoreSpawnElasped = CrystalsCoreSpawnDuration;
        Phase = MagePhase.Phase_1;
        AttackInterval = GetRandomAttackInterval();
        AttackCurrentElasped = Time.time;
        CallingEnemiesElasped = CallingEnemiesInterval;
    }

    public float GetRandomAttackInterval()
    {
        float AttackOpp = Random.Range(OriginalAttackInterval - 1f, OriginalAttackInterval + 1f);
        return AttackOpp;
    }

    public float GetRandomSpawnCrystalsInterval()
    {
        float AttackOpp = Random.Range(OriginalCrystalsCoreSpawnDuration - 3f, OriginalCrystalsCoreSpawnDuration + 5f);
        return AttackOpp;
    }
}
