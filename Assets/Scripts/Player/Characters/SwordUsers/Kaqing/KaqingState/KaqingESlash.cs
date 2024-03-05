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
        Kaqing.GetPlayerManager().GetPlayerMovementState().ResetVelocity();
        Kaqing.GetModel().SetActive(true);
        Kaqing.UpdateDefaultPosOffsetAndZoom(0f);
        Kaqing.StartElementalTimer();

        GetPlayerCharacterState().GetPlayerCharacters().PlayRandomSkillsRecastVoice();
    }

    public override void OnAnimationTransition()
    {
        Kaqing.GetPlayerManager().GetPlayerMovementState().ResetVelocity();
        GetKaqingState().ChangeState(GetKaqingState().swordIdleState);
        Kaqing.GetPlayerManager().GetCharacterRB().useGravity = true;
    }

    public override void Exit()
    {
        StopAnimation("ESlash");
    }
}
