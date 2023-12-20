using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public Vector3 Direction;
    public float timeToReachTargetRotation { get; } = 0.14f;
    public float dampedTargetRotationCurrentVelocity;
    public float dampedTargetRotationPassedTime;
    public Quaternion CurrentTargetRotation, Target_Rotation;
    public float SpeedModifier { get; set; } = 1f;
    public float StartDashTime;
    public int consecutiveDashesUsed;
    public int ConsecutiveDashesLimitAmount { get; } = 2;
    public float TimeToBeConsideredConsecutive { get; } = 1f;
    public float DashLimitReachedCooldown { get; } = 1.75f;
    public float DashLimitReachedElasped;

    public float CurrentJumpForceXZ;
    public Vector3 PreviousPosition;

    public float HitDistance;
}
