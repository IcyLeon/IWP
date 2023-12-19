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
        GetAmberState().GetAmber().Spawn4Arrows();
        GetAmberState().ChangeState(GetAmberState().amberDodgingState);
    }
}
