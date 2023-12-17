using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingESlash : KaqingElementalSkillState
{
    public KaqingESlash(KaqingState kaqingState) : base(kaqingState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("ESlash");
        GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerMovementState().ResetVelocity();
        GetKaqingState().GetKaqing().UpdateDefaultPosOffsetAndZoom(0f);
    }

    public override void OnAnimationTransition()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingIdleState);

        GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerController().GetPlayerState().ChangeState(
            GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerController().GetPlayerState().playerStayAirborneState
            );
        GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerController().GetPlayerState().playerStayAirborneState.TurnOffAirborne();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("ESlash");
        GetKaqingState().GetKaqing().StartElementalTimer(GetKaqingState().KaqingData.ElectroInfusionTimer);
    }
}
