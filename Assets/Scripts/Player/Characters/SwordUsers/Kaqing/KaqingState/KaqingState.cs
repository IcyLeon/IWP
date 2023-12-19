using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingState : SwordCharacterState
{
    public KaqingData KaqingData;
    public KaqingBurstState kaqingBurstState { get; }
    public KaqingTeleportState kaqingTeleportState { get; }
    public KaqingThrowState kaqingThrowState { get; }
    public KaqingESlash kaqingESlash { get; }
    public KaqingAimState kaqingAimState { get; }


    public Kaqing GetKaqing()
    {
        return (Kaqing)GetPlayerCharacters();
    }

    public KaqingState(Characters Characters) : base(Characters)
    {
        KaqingData = new KaqingData();
        kaqingESlash = new KaqingESlash(this);
        kaqingAimState = new KaqingAimState(this);
        kaqingBurstState = new KaqingBurstState(this);
        kaqingTeleportState = new KaqingTeleportState(this);
        swordIdleState = new KaqingIdleState(this);
        kaqingThrowState = new KaqingThrowState(this);

        ChangeState(swordIdleState);
    }
}
