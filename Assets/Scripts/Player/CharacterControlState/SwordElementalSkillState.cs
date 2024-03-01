using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordElementalSkillState : PlayerElementalSkillState
{
    public SwordElementalSkillState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ResetAllAttacks();
    }

    protected override void ResetAllAttacks()
    {
        PlayerCharacters pc = GetPlayerCharacterState().GetPlayerCharacters();
        if (pc != null)
        {
            pc.GetPlayerCharacterState().ResetBasicAttacks();
        }
    }
    public override void OnAnimationTransition()
    {
        GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordIdleState);
    }
}
