using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordIdleState : SwordControlState
{
    public SwordIdleState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void ChargeTrigger()
    {
        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing())
            return;

        GetSwordCharactersState().GetSwordCharacters().LaunchBasicAttack();
    }

}
