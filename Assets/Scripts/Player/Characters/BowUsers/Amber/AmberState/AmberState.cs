using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberState : BowCharactersState
{
    public Amber GetAmber()
    {
        return (Amber)GetPlayerCharacters();
    }
    public AmberElementalBurstState amberElementalBurstState;
    public AmberElementalSkillState amberElementalSkillState { get; }
    public AmberState(Characters Characters) : base(Characters)
    {
        bowIdleState = new AmberIdleState(this);
        amberElementalBurstState = new AmberElementalBurstState(this);
        amberElementalSkillState = new AmberElementalSkillState(this);

        ChangeState(bowIdleState);
    }
}
