using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerMovementState
{
    private float AimSpeed;
    public PlayerAimState(PlayerState playerState) : base(playerState)
    {
        AimSpeed = GetWalkSpeed() / 2f;
    }
    public override void Enter()
    {
        base.Enter();
        SetSpeed(AimSpeed);
    }

    public override void FixedUpdate()
    {
        Float();

        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }

        if (GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter() is Kaqing)
        {
            return;
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
        OnDashInput();
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
        ResetSpeed();
    }

}
