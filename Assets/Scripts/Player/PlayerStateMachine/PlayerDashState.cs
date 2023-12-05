using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerGroundState
{
    private Vector3 DashDirection;
    private float StartDashTime;
    private float DashTimer;
    private float DashElasped;

    public PlayerDashState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        DashTimer = 0.3f;
        DashElasped = 0f;

        playerStateEnum = PlayerStateEnum.DASH;
        DashDirection = GetPlayerState().GetPlayerController().transform.forward;
        Dash();
        StartDashTime = Time.time;
    }

    private void Dash()
    {
        if (Time.time - StartDashTime > GetPlayerState().PlayerData.TimeToBeConsideredConsecutive)
        {
            GetPlayerState().PlayerData.consecutiveDashesUsed = 0;
        }
        GetPlayerState().PlayerData.consecutiveDashesUsed++;

        if (GetPlayerState().PlayerData.Direction != Vector3.zero)
        {
            DashDirection = GetPlayerState().PlayerData.Direction;
        }

        DashDirection.y = 0;
        DashDirection.Normalize();
        SetTargetRotation(Quaternion.LookRotation(DashDirection));

        rb.velocity = DashDirection * 10f;

        if (GetPlayerState().PlayerData.consecutiveDashesUsed == GetPlayerState().PlayerData.ConsecutiveDashesLimitAmount)
        {
            GetPlayerState().PlayerData.consecutiveDashesUsed = 0;
            DisableDash();
        }

    }

    public override void Exit()
    {
        base.Exit();
        DashElasped = 0;
    }

    public override void Update()
    {
        base.Update();

        if (DashElasped > DashTimer)
        {
            if (GetInputDirection() == Vector3.zero)
            {
                GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
                return;
            }

            GetPlayerState().ChangeState(GetPlayerState().playerSprintState);
        }
        DashElasped += Time.deltaTime;
    }

    public override void FixedUpdate()
    {
        RotateTowardsTargetRotation();
        Float();
    }
}
