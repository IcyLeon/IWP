using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingControlState : PlayerControlState
{

    public KaqingControlState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public KaqingState GetKaqingState()
    {
        return (KaqingState)GetPlayerCharacterState();
    }

    protected bool CanTriggerESlash()
    {
        if (GetKaqingState().KaqingData.kaqingTeleporter == null)
            return false;

        return !GetKaqingState().KaqingData.kaqingTeleporter.GetEnergyOrbMoving();
    }
}
