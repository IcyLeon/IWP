using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCharactersState : PlayerCharacterState
{
    public BowAimState bowAimState { get; }
    public BowIdleState bowIdleState;

    public BowCharacters GetBowCharacters()
    {
        return (BowCharacters)GetPlayerCharacters();
    }

    public BowControlState GetBowControlState()
    {
        BowControlState p = GetIPlayerCharactersState() as BowControlState;
        return p;
    }

    public BowCharactersState(Characters Characters) : base(Characters)
    {
        CommonCharactersData = new BowData(1);
        bowAimState = new BowAimState(this);
    }

    public BowData GetBowData()
    {
        BowData bowData = CommonCharactersData as BowData;
        return bowData;
    }
}
