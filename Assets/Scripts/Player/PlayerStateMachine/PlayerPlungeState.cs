using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlungeState : PlayerAirborneState
{
    public PlayerPlungeState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StayAfloatFor(0.45f);
        playerStateEnum = PlayerStateEnum.PLUNGE;
    }
    public override void Exit()
    {
        base.Exit();
        OnPlungeAttack();
    }

    private void OnPlungeAttack()
    {
        GetPlayerState().GetPlayerController().CallPlungeAtk(GetCapsuleCollider().bounds.min);
    }

    public override void FixedUpdate()
    {
        if (!GetCanFloat())
        {
            rb.AddForce(Vector3.down * 50f, ForceMode.Acceleration);
            LimitFallVelocity();
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (IsTouchingTerrain())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
        }
    }

}
