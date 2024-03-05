using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintState : PlayerMovingState
{
    private float RunSpeed;
    public PlayerSprintState(PlayerState playerState) : base(playerState)
    {
        RunSpeed = GetWalkSpeed() * 1.45f;
    }

    public override void Enter()
    {
        base.Enter();
        SetSpeed(RunSpeed);
    }

    public override float GetAnimationSpeed()
    {
        return 1f;
    }

    public override void Exit()
    {
        base.Exit();
        ResetSpeed();
    }

    public override void Update()
    {
        base.Update();

        StopSprinting();
    }

    private void StopSprinting()
    {
        float ActualSprint = Time.deltaTime * GetPlayerState().GetPlayerManager().GetStaminaManager().GetStaminaSO().SprintCostPerSec;

        if (GetPlayerState().PlayerData.Direction == Vector3.zero)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerStoppingState);
            return;
        }

        if (!GetPlayerState().GetPlayerManager().GetStaminaManager().CanPerformStaminaAction(ActualSprint))
        {
            GetPlayerState().ChangeState(GetPlayerState().playerRunState);
            return;
        }

        GetPlayerState().GetPlayerManager().GetStaminaManager().PerformStaminaAction(ActualSprint);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
