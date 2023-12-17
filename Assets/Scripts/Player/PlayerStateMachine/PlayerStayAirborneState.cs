using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStayAirborneState : PlayerAirborneState
{
    public PlayerStayAirborneState(PlayerState playerState) : base(playerState)
    {
    }

    public void StayAirborneFor(float sec)
    {
        StayAfloatFor(sec);
    }

    public override void Enter()
    {
        base.Enter();
        rb.useGravity = false;
    }

    protected override void FloatAbove()
    {
    }

    public void TurnOffAirborne()
    {
        rb.useGravity = true;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();

        if (IsMovingDown(0.15f) && rb.useGravity)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerFallingState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
