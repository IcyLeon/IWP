using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCharactersState : PlayerCharacterState
{
    public BowData BowData;
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
        BowData = new BowData();
        bowAimState = new BowAimState(this);
    }
}
