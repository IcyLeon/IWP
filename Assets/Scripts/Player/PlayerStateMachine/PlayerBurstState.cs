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
    }

    public override void FixedUpdate()
    {
        //base.FixedUpdate();
        Float();
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }

    public override void Update()
    {
        base.Update();
        if (!IsBurstActive())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
