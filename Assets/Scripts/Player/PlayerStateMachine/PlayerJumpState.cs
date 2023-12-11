using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirborneState
{
    private bool shouldKeepRotating;
    private float JumpForce;

    public PlayerJumpState(PlayerState playerState) : base(playerState)
    {
        JumpForce = 10f;
    }
    public override void Enter()
    {
        base.Enter();
        ResetSpeed();
        SetSpeedModifier(0f);
        playerStateEnum = PlayerStateEnum.JUMP;

        ResetVelocity();
        shouldKeepRotating = GetInputDirection() != Vector3.zero;
        Jump();
    }

    private void Jump()
    {
        Vector3 direction = GetTargetRotationDirection(GetPlayerState().PlayerData.CurrentTargetRotation.eulerAngles.y);

        if (shouldKeepRotating)
        {
            UpdateInputTargetQuaternion();
        }

        Vector3 JumpForceDir = GetPlayerState().PlayerData.CurrentJumpForceXZ * direction.normalized;
        JumpForceDir.y = JumpForce;
        rb.AddForce(JumpForceDir, ForceMode.VelocityChange);
    }

    public override void FixedUpdate()
    {
        if (shouldKeepRotating)
        {
            UpdateTargetRotation();
        }
        if (IsMovingUp())
        {
            DecelerateVertically();
        }
    }

    public override void Update()
    {
        base.Update();

        if (!IsMovingDown(0f))
        {
            return;
        }

        GetPlayerState().ChangeState(GetPlayerState().playerFallingState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
