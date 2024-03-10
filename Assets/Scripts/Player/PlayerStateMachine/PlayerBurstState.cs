using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBurstState : PlayerGroundState
{
    public PlayerBurstState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ResetVelocity();
        StartAnimation("isBurst");

        if (playerCharacter)
            playerCharacter.ToggleAimCamera(false);
    }

    public override void FixedUpdate()
    {
        Float();
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }

    public override void Update()
    {
        if (!IsBurstActive())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isBurst");
    }
}
