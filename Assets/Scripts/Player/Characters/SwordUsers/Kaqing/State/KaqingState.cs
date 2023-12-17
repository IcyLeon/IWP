using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingState : CharacterState
{
    private Kaqing Kaqing;

    public KaqingData KaqingData;
    public Kaqing GetKaqing()
    {
        return Kaqing;
    }
    public KaqingBurstState kaqingBurstState { get; }
    public KaqingTeleportState kaqingTeleportState { get; }
    public KaqingThrowState kaqingThrowState { get; }
    public KaqingESlash kaqingESlash { get; }
    public KaqingAimState kaqingAimState { get; }
    public KaqingIdleState kaqingIdleState { get; }

    public void UpdateOffline()
    {
        ((IPlayerCharactersState)currentState).UpdateOffline();
    }

    public KaqingControlState GetKaqingControlState()
    {
        KaqingControlState p = currentState as KaqingControlState;

        return p;
    }
    public KaqingState(Kaqing kaqing)
    {
        Kaqing = kaqing;
        KaqingData = new KaqingData();
        kaqingESlash = new KaqingESlash(this);
        kaqingAimState = new KaqingAimState(this);
        kaqingBurstState = new KaqingBurstState(this);
        kaqingTeleportState = new KaqingTeleportState(this);
        kaqingIdleState = new KaqingIdleState(this);
        kaqingThrowState = new KaqingThrowState(this);

        ChangeState(kaqingIdleState);
    }
}
