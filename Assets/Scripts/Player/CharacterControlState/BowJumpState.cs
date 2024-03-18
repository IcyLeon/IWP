using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowJumpState : BowControlState
{
    public override void Enter()
    {
        base.Enter();
        GetPlayerCharacterState().GetPlayerCharacters().PlayRandomJumpVoice();
    }

    public override void Update()
    {
        base.Update();
        UpdateJumpState();
    }

    private void UpdateJumpState()
    {
        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsJumping())
            return;

        GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
    }

    public BowJumpState(PlayerCharacterState pcs) : base(pcs)
    {
    }

}
