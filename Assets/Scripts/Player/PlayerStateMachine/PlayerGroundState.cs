using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    protected override void SubscribeInput()
    {
        base.SubscribeInput();
        GetPlayerState().GetPlayerController().GetPlayerActions().Jump.started += OnJumpAction;
    }

    protected override void UnsubscribeInput()
    {
        base.UnsubscribeInput();
        GetPlayerState().GetPlayerController().GetPlayerActions().Jump.started -= OnJumpAction;
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
            GetPlayerState().ChangeState(GetPlayerState().playerBurstState);
            if (playerCharacter != null)
            {
                playerCharacter.GetPlayerCharacterState().ResetBasicAttacks();
            }
            return;
        }
        else
        {

            OnAimInput();

            if (IsAiming())
                return;

            if (GetPlayerState().GetPlayerManager().IsSkillCasting())
                return;

            if (IsMovingDown(0.15f) && !IsTouchingTerrain() && GetPlayerState().GetPlayerMovementState() is not PlayerJumpState
                && GetPlayerState().GetPlayerMovementState() is not PlayerDashState)
            {
                OnFall();
                return;
            }
        }
    }
    

    private void OnAimInput()
    {
        if (IsAiming() && this is not PlayerAimState)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerAimState);
        }
    }

    private void OnJumpAction(InputAction.CallbackContext CallbackContext)
    {
        if (IsAttacking())
            return;

        GetPlayerState().ChangeState(GetPlayerState().playerJumpState);
    }
}
