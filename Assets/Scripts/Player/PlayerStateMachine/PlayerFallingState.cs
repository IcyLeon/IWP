using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerAirborneState
{
    public PlayerFallingState(PlayerState playerState) : base(playerState)
    {
    }

    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void FixedUpdate()
    {
        UpdateTargetRotation();
        LimitFallVelocity();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (IsTouchingTerrain())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerLandingState);
            return;
        }
    }

}
