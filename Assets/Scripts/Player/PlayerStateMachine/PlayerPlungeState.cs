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
        ResetVelocity();
        rb.useGravity = false;
        StartAnimation("isPlunging");
    }
    public override void Exit()
    {
        base.Exit();
        rb.useGravity = true;
        OnPlungeAttack();
        StopAnimation("isPlunging");
    }
    private void OnPlungeAttack()
    {
        Vector3 hitpos = GetCapsuleCollider().bounds.center + Vector3.down * (GetPlayerState().GetPlayerController().GetResizeableCollider().GetDefaultColliderData_height() * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepHeightPercentage + GetCapsuleCollider().height / 2f - GetCapsuleCollider().radius / 2f);
        GetPlayerState().GetPlayerController().CallPlungeAtk(hitpos);
        
        GetPlayerState().GetPlayerController().GetCameraManager().CameraShake(2.5f, 2.5f, 1.5f);
    }

    public override void OnAnimationTransition()
    {
        rb.useGravity = true;
    }

    public override void FixedUpdate()
    {
        Float();
        if (rb.useGravity)
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
            return;
        }
    }

}
