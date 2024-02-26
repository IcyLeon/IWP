using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingAimState : KaqingElementalSkillState
{
    private float MaxTimer = 5f;
    private float Timer;
    public KaqingAimState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Timer = 0f;
        GetKaqingState().GetKaqing().GetSwordModel().gameObject.SetActive(false);
    }

    public override void ElementalSkillTrigger()
    {
        GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
    }

    public override void Update()
    {
        base.Update();

        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

        GetPlayerCharacterState().GetPlayerCharacters().UpdateCameraAim();
        GetKaqingState().GetKaqing().InitElementalSkillHitPos_Aim();
        GetKaqingState().GetKaqing().UpdateTargetOrb();

        if (Timer >= MaxTimer)
        {
            GetKaqingState().ChangeState(GetKaqingState().kaqingThrowState);
            return;
        }
        Timer += Time.deltaTime;
    }

    public override void Exit()
    {
        GetKaqingState().GetKaqing().DestroyTargetOrb();
    }
}
