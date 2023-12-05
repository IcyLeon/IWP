using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirborneState
{
    private bool StartFalling;
    public PlayerJumpState(PlayerState playerState) : base(playerState)
    {
    }
    public override void Enter()
    {
        base.Enter();
        playerStateEnum = PlayerStateEnum.JUMP;
        StartFalling = false;
        ResetVelocity();
        rb.AddForce(8f * Vector3.up, ForceMode.VelocityChange);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }

    public override void Update()
    {
        base.Update();
        if (!StartFalling && IsMovingUp(0f))
        {
            StartFalling = true;
        }

        if (!StartFalling || IsMovingUp(0f))
        {
            return;
        }

        GetPlayerState().ChangeState(GetPlayerState().playerFallingState);
    }

    public override void Exit()
    {
        base.Exit();

        StartFalling = false;
    }
}
