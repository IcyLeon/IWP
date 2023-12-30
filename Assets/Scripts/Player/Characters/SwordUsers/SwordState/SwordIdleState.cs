using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordIdleState : SwordControlState
{
    public SwordIdleState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0);
    }
    public override void ChargeTrigger()
    {
        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing() || GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

        GetSwordCharactersState().GetSwordCharacters().LaunchBasicAttack();
    }

}
