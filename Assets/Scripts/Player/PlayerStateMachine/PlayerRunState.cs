using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerMovingState
{
    public PlayerRunState(PlayerState playerState) : base(playerState)
    {
    }

    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
        ResetSpeedModifier();
        playerStateEnum = PlayerStateEnum.WALK;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
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
