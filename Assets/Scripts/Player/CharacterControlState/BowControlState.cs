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
        GetBowCharactersState().GetBowData().CurrentElemental = Elemental.NONE;
        GetBowCharactersState().GetBowData().ChargeElapsed = 0;
    }

    public BowControlState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
