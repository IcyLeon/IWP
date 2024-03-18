using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotData
{
    public float OriginalAttackInterval = 0.4f, AttackInterval;
    public float AttackCurrentElasped = 0f;

    public float timeToReachTargetRotation { get; } = 0.14f;
    public float dampedTargetRotationCurrentVelocity;
    public float dampedTargetRotationPassedTime;
    public Quaternion CurrentTargetRotation;

    public PunchingBotData()
    {
        AttackInterval = GetRandomAttackInterval();
    }

    public float GetRandomAttackInterval()
    {
        float AttackOpp = Random.Range(OriginalAttackInterval - 0.2f, OriginalAttackInterval + 0.1f);
        return AttackOpp;
    }

}
