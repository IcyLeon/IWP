using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingThrowState : KaqingElementalSkillState
{
    public KaqingThrowState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isThrowing");
        GetKaqingState().GetKaqing().GetSwordModel().gameObject.SetActive(false);
    }

    public override void OnAnimationTransition()
    {
        GetKaqingState().ChangeState(GetKaqingState().swordIdleState);
        GetPlayerCharacterState().GetPlayerCharacters().PlayRandomSkillsVoice();
    }

    public override void Exit()
    {
        StopAnimation("isThrowing");
        GetKaqingState().GetKaqing().GetSwordModel().gameObject.SetActive(true);
        GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0.2f);
    }
}
