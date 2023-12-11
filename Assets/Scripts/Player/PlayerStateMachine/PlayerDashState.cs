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
        DashTimer = 0.25f;
        DashElasped = 0f;
        GetPlayerState().PlayerData.CurrentJumpForceXZ = 5f;
        playerStateEnum = PlayerStateEnum.DASH;
        DashDirection = GetPlayerState().GetPlayerController().transform.forward;
        Dash();
        StartDashTime = Time.time;
    }

    //private void SpawnDash()
    //{
    //    AssetManager.GetInstance().SpawnDash(GetPlayerState().GetPlayerController().GetPlayerOffsetPosition().position, GetPlayerState().GetPlayerController().transform.rotation);
    //}
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

        if (!CheckIfisAboutToFall())
            rb.velocity = DashDirection * 10f;

        if (GetPlayerState().PlayerData.consecutiveDashesUsed == GetPlayerState().PlayerData.ConsecutiveDashesLimitAmount)
        {
            GetPlayerState().PlayerData.consecutiveDashesUsed = 0;
            DisableDash();
        }
        GetPlayerState().GetPlayerController().GetStaminaManager().PerformStaminaAction(GetPlayerState().GetPlayerController().GetStaminaManager().GetStaminaSO().DashCost);
    }

    public override void Exit()
    {
        base.Exit();

        DashElasped = 0;
    }

    public override void Update()
    {
        base.Update();

        if (CheckIfisAboutToFall())
            ResetVelocity();

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
