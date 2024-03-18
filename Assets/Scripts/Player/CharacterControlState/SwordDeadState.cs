using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDeadState : CharacterDeadState
{

    public SwordDeadState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    protected override void DeadUpdate()
    {
        base.DeadUpdate();

        if (GetPlayerCharacterState().GetPlayerCharacters().IsDead())
            return;

        GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordIdleState);
    }
}
