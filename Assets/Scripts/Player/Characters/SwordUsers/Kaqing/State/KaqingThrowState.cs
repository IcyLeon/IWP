using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingThrowState : KaqingElementalSkillState
{
    public KaqingThrowState(KaqingState kaqingState) : base(kaqingState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isThrowing");
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
        GetKaqingState().GetKaqing().UpdateDefaultPosOffsetAndZoom(0.2f);
    }
}
