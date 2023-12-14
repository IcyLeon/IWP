using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerGroundState : PlayerMovementState
{
    public PlayerGroundState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected void DisableDash()
    {
        GetPlayerState().PlayerData.DashLimitReachedElasped = GetPlayerState().PlayerData.DashLimitReachedCooldown;
    }

    private bool CanDash()
    {
        return GetPlayerState().PlayerData.DashLimitReachedElasped <= 0;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Float();

        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }

    public void OnFall()
    {
        GetPlayerState().ChangeState(GetPlayerState().playerFallingState);
    }

    public override void Update()
    {
        base.Update();

        if (IsBurstActive())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerBurstState);
            return;
        }
        else
        {
            OnJumpInput();
            OnDashInput();
            OnAimInput();

            if (IsMovingDown(0.15f) && !IsTouchingTerrain() && GetPlayerState().GetPlayerMovementState() is not PlayerDashState
                && GetPlayerState().GetPlayerMovementState() is not PlayerJumpState)
            {
                OnFall();
                return;
            }
        }
    }
    
    private void OnJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnJump();
    }

    private void OnDashInput()
    {
        if (!GetPlayerState().GetPlayerController().GetPlayerManager().GetStaminaManager().CanPerformDash() || !CanDash())
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            OnDash();
    }

    private void OnAimInput()
    {
        if (IsAiming())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerAimState);
        }
    }

    private void OnJump()
    {
        GetPlayerState().ChangeState(GetPlayerState().playerJumpState);
    }

    private void OnDash()
    {
        GetPlayerState().ChangeState(GetPlayerState().playerDashState);
    }
}
