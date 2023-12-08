using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingState : PlayerGroundState
{
    public PlayerMovingState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();

        if (GetInputDirection() == Vector3.zero && GetPlayerState().GetPlayerMovementState() is not PlayerJumpState)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
