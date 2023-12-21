using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowElementalSkillState : PlayerElementalSkillState
{
    public BowElementalSkillState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void OnAnimationTransition()
    {
        GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
    }
}
