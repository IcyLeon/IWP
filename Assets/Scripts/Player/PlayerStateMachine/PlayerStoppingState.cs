using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStoppingState : PlayerGroundState
{
    public PlayerStoppingState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isStopping");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateTargetRotation();
    }

    public override void OnAnimationTransition()
    {
        GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
    }


    public override void Update()
    {
        base.Update();
        if (CheckIfisAboutToFall())
            ResetVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isStopping");
    }

}
