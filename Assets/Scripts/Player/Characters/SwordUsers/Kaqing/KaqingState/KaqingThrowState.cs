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
        GetKaqingState().GetKaqing().GetSwordModel().SetActive(false);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnAnimationTransition()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingIdleState);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isThrowing");
        GetKaqingState().GetKaqing().GetSwordModel().SetActive(true);
        GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0.2f);
    }
}