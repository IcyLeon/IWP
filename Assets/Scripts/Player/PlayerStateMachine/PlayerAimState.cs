using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerGroundState
{
    private float AimSpeed;
    public PlayerAimState(PlayerState playerState) : base(playerState)
    {
        AimSpeed = 1f;
    }
    public override void Enter()
    {
        base.Enter();
        StartAnimation("isAiming");
        SetSpeed(AimSpeed);
    }

    public override void FixedUpdate()
    {

        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }

        if (CheckIfisAboutToFall())
        {
            return;
        }

        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();

        if (!IsAiming())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
            return;
        }
        if (CheckIfisAboutToFall())
            ResetVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isAiming");
        ResetSpeed();
    }

}
