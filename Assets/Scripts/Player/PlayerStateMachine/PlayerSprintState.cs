using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintState : PlayerMovingState
{
    public PlayerSprintState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerStateEnum = PlayerStateEnum.SPRINTING;
        SetSpeedModifier(2f);
    }

    public override void Exit()
    {
        base.Exit();
        ResetSpeedModifier();
    }

    public override void Update()
    {
        base.Update();

        StopSprinting();
    }

    private void StopSprinting()
    {
        float ActualSprint = Time.deltaTime * GetPlayerState().GetPlayerController().GetStaminaManager().GetStaminaSO().SprintCostPerSec;

        if (GetPlayerState().PlayerData.Direction == Vector3.zero)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
            return;
        }

        if (!GetPlayerState().GetPlayerController().GetStaminaManager().CanPerformStaminaAction(ActualSprint))
        {
            GetPlayerState().ChangeState(GetPlayerState().playerRunState);
            return;
        }

        GetPlayerState().GetPlayerController().GetStaminaManager().PerformStaminaAction(ActualSprint);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
