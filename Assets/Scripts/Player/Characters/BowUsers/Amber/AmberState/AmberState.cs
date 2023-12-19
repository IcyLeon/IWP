using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberState : BowCharactersState
{
    public Amber GetAmber()
    {
        return (Amber)GetPlayerCharacters();
    }
    public AmberDodgingState amberDodgingState { get; }
    public AmberState(Characters Characters) : base(Characters)
    {
        bowIdleState = new AmberIdleState(this);
        amberDodgingState = new AmberDodgingState(this);

        ChangeState(bowIdleState);
    }
}