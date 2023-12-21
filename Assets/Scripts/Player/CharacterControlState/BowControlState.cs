using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowControlState : PlayerControlState
{
    public override void Enter()
    {
        base.Enter();
        ResetCharge();
    }

    protected void ResetCharge()
    {
        GetBowCharactersState().BowData.CurrentElemental = Elemental.NONE;
        GetBowCharactersState().BowData.ChargeElapsed = 0;
    }

    public BowControlState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
