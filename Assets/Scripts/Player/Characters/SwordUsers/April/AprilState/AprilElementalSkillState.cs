using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilElementalSkillState : SwordElementalSkillState
{
    public AprilState GetAprilState()
    {
        return (AprilState)GetSwordCharactersState();
    }

    public AprilElementalSkillState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetSwordCharactersState().GetSwordCharacters().GetSwordModel().gameObject.SetActive(false);
    }

    public override void OnAnimationTransition()
    {
        GetAprilState().GetApril().SpawnShield();
        GetAprilState().GetApril().DrainHealth();
        GetAprilState().GetPlayerCharacters().GetPlayerManager().GetPlayerElementalSkillandBurstManager().SubscribeSkillsState(GetAprilState().GetApril());
        base.OnAnimationTransition();
    }

    public override void Exit()
    {
        base.Exit();
        GetSwordCharactersState().GetSwordCharacters().GetSwordModel().gameObject.SetActive(true);

    }
}
