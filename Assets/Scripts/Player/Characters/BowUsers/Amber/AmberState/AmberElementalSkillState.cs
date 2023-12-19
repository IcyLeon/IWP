using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberElementalSkillState : PlayerElementalSkillState
{
    public BowCharactersState GetBowCharactersState()
    {
        return (BowCharactersState)GetPlayerCharacterState();
    }

    public AmberElementalSkillState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
