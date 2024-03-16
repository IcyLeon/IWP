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
        Kaqing.GetSwordModel().gameObject.SetActive(false);
    }

    public override void OnAnimationTransition()
    {
        GetKaqingState().ChangeState(GetKaqingState().swordIdleState);
        Kaqing.PlayAimSound(false);
        GetPlayerCharacterState().GetPlayerCharacters().PlayRandomSkillsVoice();
    }

    public override void Exit()
    {
        StopAnimation("isThrowing");
        Kaqing.GetSwordModel().gameObject.SetActive(true);
        GetPlayerCharacterState().GetPlayerCharacters().ToggleOffAimCameraDelay(0.2f);
    }
}
