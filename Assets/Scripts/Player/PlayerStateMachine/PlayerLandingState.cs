using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : PlayerGroundState
{
    public PlayerLandingState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ResetSpeed();
        StartAnimation("isLanding");
    }

    public override void OnAnimationTransition()
    {
        GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isLanding");
    }

}
