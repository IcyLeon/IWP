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
        base.Enter();
        ResetAllBasicAtk();
        StartAnimation("isDashing");
        GetPlayerState().PlayerData.CurrentJumpForceXZ = 5f;
        DashDirection = GetPlayerState().GetPlayerManager().transform.forward;
        Dash();
        GetPlayerState().PlayerData.StartDashTime = Time.time;
    }

    //private void SpawnDash()
    //{
    //    AssetManager.GetInstance().SpawnDash(GetPlayerState().GetPlayerController().GetPlayerOffsetPosition().position, GetPlayerState().GetPlayerController().transform.rotation);
    //}
    private void Dash()
    {
        if (playerCharacter != null)
            playerCharacter.ToggleAimCamera(false);

        if (Time.time - GetPlayerState().PlayerData.StartDashTime > PlayerData.TimeToBeConsideredConsecutive)
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
            GetPlayerState().rb.velocity = DashDirection * 10f;

        if (GetPlayerState().PlayerData.consecutiveDashesUsed == PlayerData.ConsecutiveDashesLimitAmount)
        {
            GetPlayerState().PlayerData.consecutiveDashesUsed = 0;
            DisableDash();
        }
        GetPlayerState().GetPlayerManager().GetStaminaManager().PerformStaminaAction(GetPlayerState().GetPlayerManager().GetStaminaManager().GetStaminaSO().DashCost);
    }

    private void DisableDash()
    {
        GetPlayerState().GetPlayerController().DisableInput(GetPlayerState().GetPlayerController().GetPlayerActions().Dash, true, GetPlayerState().PlayerData.DashLimitReachedCooldown);
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
            GetPlayerState().rb.velocity = GetPlayerState().PlayerData.Direction * GetPlayerState().rb.velocity.magnitude;
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
        PlayerCharacters pc = GetPlayerState().GetPlayerManager().GetCurrentCharacter();
        if (pc != null)
        {
            pc.GetPlayerCharacterState().ResetBasicAttacks();
        }
    }
    public override void FixedUpdate()
    {
        Float();
        UpdateInputTargetQuaternion();
        UpdateTargetRotation();
    }
}
