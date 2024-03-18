using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElementalSkillState : PlayerControlState
{
    public PlayerElementalSkillState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    protected virtual void ResetAllAttacks()
    {

    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isCasting");
        GetPlayerCharacterState().GetPlayerCharacters().PlayRandomSkillsVoice();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isCasting");
    }
}
