using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCharacterState : PlayerCharacterState
{
    public BowAimState bowAimState;
    public BowIdleState bowIdleState;
    public BowJumpState bowJumpState;

    public BowCharacters GetBowCharacters()
    {
        return (BowCharacters)GetPlayerCharacters();
    }

    public BowControlState GetBowControlState()
    {
        BowControlState p = GetIPlayerCharactersState() as BowControlState;
        return p;
    }

    public BowCharacterState(Characters Characters) : base(Characters)
    {
        CommonCharactersData = new BowData(1);
        characterDeadState = new BowDeadState(this);
        bowJumpState = new BowJumpState(this);
        bowAimState = new BowAimState(this);
    }

    public BowData GetBowData()
    {
        BowData bowData = CommonCharactersData as BowData;
        return bowData;
    }
}
