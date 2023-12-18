using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingState : PlayerCharacterState
{
    public KaqingData KaqingData;
    public KaqingBurstState kaqingBurstState { get; }
    public KaqingTeleportState kaqingTeleportState { get; }
    public KaqingThrowState kaqingThrowState { get; }
    public KaqingESlash kaqingESlash { get; }
    public KaqingAimState kaqingAimState { get; }
    public KaqingIdleState kaqingIdleState { get; }

    public KaqingControlState GetKaqingControlState()
    {
        KaqingControlState p = GetIPlayerCharactersState() as KaqingControlState;

        return p;
    }

    public Kaqing GetKaqing()
    {
        return (Kaqing)Characters;
    }

    public KaqingState(Characters Characters) : base(Characters)
    {
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
