using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingState : SwordCharacterState
{
    public KaqingBurstState kaqingBurstState;
    public KaqingTeleportState kaqingTeleportState;
    public KaqingThrowState kaqingThrowState;
    public KaqingESlash kaqingESlash;
    public KaqingAimState kaqingAimState;
    public KaqingAttackState kaqingAttackState;

    public Kaqing GetKaqing()
    {
        return (Kaqing)GetPlayerCharacters();
    }

    public KaqingData KaqingData { get 
        { return (KaqingData)CommonCharactersData; }
    }


    public KaqingState(Characters Characters) : base(Characters)
    {
        CommonCharactersData = new KaqingData(4);
        kaqingESlash = new KaqingESlash(this);
        kaqingAimState = new KaqingAimState(this);
        kaqingBurstState = new KaqingBurstState(this);
        kaqingTeleportState = new KaqingTeleportState(this);
        swordIdleState = new KaqingIdleState(this);
        kaqingThrowState = new KaqingThrowState(this);
        kaqingAttackState = new KaqingAttackState(this);
        ChangeState(swordIdleState);
    }
}
