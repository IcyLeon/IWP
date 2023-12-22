using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilElementalBurstState : SwordElementalBurstState
{
    public AprilState GetAprilState()
    {
        return (AprilState)GetSwordCharactersState();
    }

    public AprilElementalBurstState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetSwordCharactersState().GetSwordCharacters().GetSwordModel().gameObject.SetActive(false);
    }

    public override void Exit()
    {
        base.Exit();
        GetSwordCharactersState().GetSwordCharacters().GetSwordModel().gameObject.SetActive(true);

    }
}
