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
        }
        else
        {
            OnJumpInput();
            OnDashInput();
            OnAimInput();

            if (IsMovingDown(0.15f) && !IsTouchingTerrain() && GetPlayerState().GetPlayerMovementState() is not PlayerDashState 
                && GetPlayerState().GetPlayerMovementState() is not PlayerJumpState)
                OnFall();
        }
    }
    
    private void OnJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnJump();
    }

    private void OnDashInput()
    {
        if (!GetPlayerState().GetPlayerController().GetStaminaManager().CanPerformDash() || !CanDash())
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            OnDash();
    }

    private void OnAimInput()
    {
        if (IsAiming())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerAimState);
            return;
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
