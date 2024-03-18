using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordJumpState : SwordControlState
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

        GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordIdleState);
    }

    public SwordJumpState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
