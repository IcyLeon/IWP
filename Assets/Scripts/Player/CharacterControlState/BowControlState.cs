using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowControlState : PlayerControlState
{
    public BowData GetBowData()
    {
        if (GetBowCharactersState() == null)
            return null;

        return GetBowCharactersState().GetBowData();
    }
    public override void Enter()
    {
        base.Enter();
        GetPlayerCharacterState().GetPlayerCharacters().ToggleOffAimCameraDelay(0);
        ResetCharge();
    }

    protected void ResetCharge()
    {
        GetBowData().CurrentElemental = Elemental.NONE;
        GetBowData().ChargeElapsed = 0;
    }

    public BowControlState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
