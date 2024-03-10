using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordElementalBurstState : PlayerElementalBurstState
{
    public SwordElementalBurstState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordIdleState);
    }
}
