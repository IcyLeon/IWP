using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowElementalBurstState : PlayerElementalBurstState
{
    public BowElementalBurstState(PlayerCharacterState pcs) : base(pcs)
    {
    }
    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
    }
}
