using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingAimState : KaqingControlState
{
    public KaqingAimState(KaqingState kaqingState) : base(kaqingState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetKaqingState().GetKaqing().GetSwordModel().SetActive(false);
        StartAnimation("2ndSkillAim");
    }

    public override void ElementalSkillHold()
    {
        GetKaqingState().GetKaqing().UpdateCameraAim();
    }

    public override void ElementalSkillTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
    }

    public override void Update()
    {
        base.Update();

        GetKaqingState().GetKaqing().InitElementalSkillHitPos_Aim();
        GetKaqingState().GetKaqing().UpdateTargetOrb();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("2ndSkillAim");
        GetKaqingState().GetKaqing().DestroyTargetOrb();
        GetKaqingState().KaqingData.threasHold_Charged = 0f;
    }
}
