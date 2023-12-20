using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberDodgingState : AmberElementalSkillState
{
    public AmberDodgingState(PlayerCharacterState pcs) : base(pcs)
    {
    }
    public override void Enter()
    {
        base.Enter();
        StartAnimation("isDodging");
    }

    public override void OnAnimationTransition()
    {
        GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation("isDodging");
    }
}
