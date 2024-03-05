using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingElementalSkillState : SwordElementalSkillState
{
    protected Kaqing Kaqing;
    protected KaqingState GetKaqingState()
    {
        return (KaqingState)GetSwordCharactersState();
    }

    public override void Enter()
    {
        ResetAllAttacks();
    }

    public KaqingElementalSkillState(PlayerCharacterState pcs) : base(pcs)
    {
        Kaqing = GetKaqingState().GetKaqing();
    }
}
