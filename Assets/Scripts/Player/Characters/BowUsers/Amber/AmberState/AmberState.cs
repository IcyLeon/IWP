using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberState : BowCharacterState
{
    public Amber GetAmber()
    {
        return (Amber)GetPlayerCharacters();
    }
    public AmberElementalBurstState amberElementalBurstState;
    public AmberElementalSkillState amberElementalSkillState;
    public AmberAttackState amberAttackState;
    public AmberState(Characters Characters) : base(Characters)
    {
        bowIdleState = new AmberIdleState(this);
        amberElementalBurstState = new AmberElementalBurstState(this);
        amberElementalSkillState = new AmberElementalSkillState(this);
        amberAttackState = new AmberAttackState(this);
        ChangeState(bowIdleState);
    }
}
