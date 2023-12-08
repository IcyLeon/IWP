using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ResetSpeed();
        GetPlayerState().PlayerData.CurrentJumpForceXZ = 0f;
        playerStateEnum = PlayerStateEnum.IDLE;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (GetInputDirection() != Vector3.zero && GetPlayerState().GetPlayerMovementState() is not PlayerJumpState)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerRunState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
