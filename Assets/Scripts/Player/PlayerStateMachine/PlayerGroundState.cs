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

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Float();

        if (IsMovingHorizontally() && this is not PlayerDashState)
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
            PlayerCharacters playerCharacter = GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter();
            if (playerCharacter != null)
            {
                playerCharacter.ResetAttack();
            }
            GetPlayerState().ChangeState(GetPlayerState().playerBurstState);
            return;
        }
        else
        {
            if (GetPlayerState().GetPlayerController().GetPlayerManager().IsSkillCasting())
                return;

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
}
