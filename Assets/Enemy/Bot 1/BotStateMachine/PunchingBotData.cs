using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBotData
{
    public float AttackInterval = 0.5f;
    public float AttackCurrentElasped = 0f;

    public float timeToReachTargetRotation { get; } = 0.14f;
    public float dampedTargetRotationCurrentVelocity;
    public float dampedTargetRotationPassedTime;
    public Quaternion CurrentTargetRotation, Target_Rotation;

    public PunchingBotData()
    {
        AttackInterval = GetRandomAttackInterval();
    }

    public float GetRandomAttackInterval()
    {
        float AttackOpp = Random.Range(AttackInterval - 0.35f, AttackInterval + 0.2f);
        return AttackOpp;
    }

}
