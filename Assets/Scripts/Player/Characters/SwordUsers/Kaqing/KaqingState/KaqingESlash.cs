using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingESlash : KaqingElementalSkillState
{
    public KaqingESlash(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("ESlash");
        GetKaqingState().GetKaqing().GetPlayerManager().GetPlayerMovementState().ResetVelocity();
        GetKaqingState().GetKaqing().GetModel().SetActive(true);
        GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0f);
        GetKaqingState().GetKaqing().StartElementalTimer();
    }

    public override void OnAnimationTransition()
    {
        GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState().ResetVelocity();
        GetKaqingState().ChangeState(GetKaqingState().swordIdleState);
        GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().GetCharacterRB().useGravity = true;
    }

    public override void Exit()
    {
        StopAnimation("ESlash");
    }
}
