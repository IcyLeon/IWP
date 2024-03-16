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
        GetPlayerState().rb.useGravity = false;
        GetPlayerState().GetPlayerController().GetCapsuleCollider().isTrigger = true;
        StartAnimation("isPlunging");
    }
    public override void Exit()
    {
        base.Exit();
        GetPlayerState().rb.useGravity = true;
        GetPlayerState().GetPlayerController().GetCapsuleCollider().isTrigger = false;
        OnPlungeAttack();
        StopAnimation("isPlunging");
    }
    private void OnPlungeAttack()
    {
        Vector3 hitpos = GetCapsuleCollider().bounds.center + Vector3.down * (GetPlayerState().GetPlayerController().GetResizeableCollider().GetDefaultColliderData_height() * GetPlayerState().GetPlayerController().GetResizeableCollider().GetSlopeData().StepHeightPercentage + GetCapsuleCollider().height / 2f - GetCapsuleCollider().radius / 2f);
        GetPlayerState().GetPlayerController().CallPlungeAtk(hitpos);
        GetPlayerState().GetPlayerController().GetCameraManager().CameraShake(3.5f, 3.5f, 1.5f);
    }

    public override void OnAnimationTransition()
    {
        GetPlayerState().rb.useGravity = true;
    }

    public override void FixedUpdate()
    {
        Float();
        if (GetPlayerState().rb.useGravity)
        {
            GetPlayerState().rb.AddForce(45f * Vector3.down, ForceMode.Acceleration);
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
