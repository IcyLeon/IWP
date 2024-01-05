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

    public float AttackInterval = 1.5f;
    public float AttackCurrentElasped = 0f;
    public int CurrentAttackIndex = 1;

    public float CurrentStunElasped = 0f;
    public float stunDuration = 6f;

    public int SpawnEnemiesAmount = 3;

    public float CrystalsCoreStayDuration = 8f;
    public float CrystalsCoreStayElasped = 0f;
    public float CrystalsCoreSpawnDuration = 15f;
    public float CrystalsCoreSpawnElasped = 0f;

    public float TakeOffCooldown = 15f;
    public float TakeOffElapsed = 0f;

    public float FireBallSpawnDuration = 3.5f;
    public float FireBallSpawnElapsed = 0f;

    public float timeToReachTargetRotation { get; } = 0.14f;
    public float dampedTargetRotationCurrentVelocity;
    public float dampedTargetRotationPassedTime;
    public Quaternion CurrentTargetRotation, Target_Rotation;


    public int NoOfFireball = 5;

    public MageEnemyData()
    {
        ShieldCurrentElasped = ShieldCooldown;
        TakeOffElapsed = TakeOffCooldown;
        FireBallSpawnElapsed = FireBallSpawnDuration;
        CrystalsCoreSpawnElasped = GetRandomSpawnCrystalsInterval();
        Phase = MagePhase.Phase_1;
        AttackInterval = GetRandomAttackInterval();
    }

    public float GetRandomAttackInterval()
    {
        float AttackOpp = Random.Range(AttackInterval - 1f, AttackInterval + 1f);
        return AttackOpp;
    }

    public float GetRandomSpawnCrystalsInterval()
    {
        float AttackOpp = Random.Range(CrystalsCoreSpawnDuration - 3f, CrystalsCoreSpawnDuration + 5f);
        return AttackOpp;
    }
}
