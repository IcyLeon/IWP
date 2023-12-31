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
        SwordCharacters sc = GetSwordCharactersState().GetSwordCharacters();
        if (sc != null)
        {
            if (sc != null)
            {
                sc.ResetBasicAttacks();
            }
            sc.ResetAttack();
        }
    }
    public override void OnAnimationTransition()
    {
        GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordIdleState);
    }
}
