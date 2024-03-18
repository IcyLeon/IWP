using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public Vector3 Direction;
    public const float timeToReachTargetRotation = 0.14f;
    public float dampedTargetRotationCurrentVelocity;
    public float dampedTargetRotationPassedTime;
    public Quaternion CurrentTargetRotation;
    public float SpeedModifier = 1f;
    public float StartDashTime;
    public int consecutiveDashesUsed;
    public const int ConsecutiveDashesLimitAmount = 2;
    public const float TimeToBeConsideredConsecutive = 1f;
    public float DashLimitReachedCooldown { get; } = 1.75f;

    public float CurrentJumpForceXZ;
    public Vector3 PreviousPosition;

    public float HitDistance;


    // common data
    public const float TimeForImmuneDamageTaken = 0.5f;
    public float TimeForImmuneDamageElapsed;
}
