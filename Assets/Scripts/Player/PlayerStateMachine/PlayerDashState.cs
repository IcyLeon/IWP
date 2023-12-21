using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerGroundState
{
    private Vector3 DashDirection;

    public PlayerDashState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        ResetAllBasicAtk();
        base.Enter();
        StartAnimation("isDashing");
        GetPlayerState().PlayerData.CurrentJumpForceXZ = 5f;
        DashDirection = GetPlayerState().GetPlayerController().transform.forward;
        Dash();
        GetPlayerState().PlayerData.StartDashTime = Time.time;
    }

    //private void SpawnDash()
    //{
    //    AssetManager.GetInstance().SpawnDash(GetPlayerState().GetPlayerController().GetPlayerOffsetPosition().position, GetPlayerState().GetPlayerController().transform.rotation);
    //}
    private void Dash()
    {
        if (Time.time - GetPlayerState().PlayerData.StartDashTime > GetPlayerState().PlayerData.TimeToBeConsideredConsecutive)
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
        GetPlayerState().GetPlayerController().GetPlayerManager().GetStaminaManager().PerformStaminaAction(GetPlayerState().GetPlayerController().GetPlayerManager().GetStaminaManager().GetStaminaSO().DashCost);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isDashing");
    }

    public override void Update()
    {
        base.Update();

        if (GetInputDirection() != Vector3.zero)
        {
            rb.velocity = GetPlayerState().PlayerData.Direction * rb.velocity.magnitude;
        }

        if (CheckIfisAboutToFall())
            ResetVelocity();
    }

    public override void OnAnimationTransition()
    {
        if (GetInputDirection() == Vector3.zero)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerStoppingState);
            return;
        }

        GetPlayerState().ChangeState(GetPlayerState().playerSprintState);
    }

    private void ResetAllBasicAtk()
    {
        PlayerCharacters pc = GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter();
        if (pc != null)
        {
            SwordCharacters sc = pc as SwordCharacters;
            if (sc != null)
            {
                sc.ResetBasicAttacks();
            }
            pc.ResetAttack();
        }
    }
    public override void FixedUpdate()
    {
        Float();
        UpdateInputTargetQuaternion();
        UpdateTargetRotation();
    }
}
