using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowDeadState : CharacterDeadState
{
    public BowDeadState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    protected override void DeadUpdate()
    {
        base.DeadUpdate();

        if (GetPlayerCharacterState().GetPlayerCharacters().IsDead())
            return;

        GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
    }
}
