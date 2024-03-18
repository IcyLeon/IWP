using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberIdleState : BowIdleState
{
    public AmberState GetAmberState()
    {
        return (AmberState)GetPlayerCharacterState();
    }

    public AmberIdleState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void ElementalSkillTrigger()
    {
        GetAmberState().ChangeState(GetAmberState().amberElementalSkillState);
    }

    public override void ElementalBurstTrigger()
    {
        GetAmberState().ChangeState(GetAmberState().amberElementalBurstState);
    }

    protected override void TransitionToAttackState()
    {
        GetAmberState().ChangeState(GetAmberState().amberAttackState);
    }
}
