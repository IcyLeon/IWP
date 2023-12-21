using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilIdleState : SwordIdleState
{
    public AprilState GetAprilState()
    {
        return (AprilState)GetPlayerCharacterState();
    }

    public AprilIdleState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void ElementalSkillTrigger()
    {
        GetAprilState().ChangeState(GetAprilState().aprilElementalSkillState);
    }
}
