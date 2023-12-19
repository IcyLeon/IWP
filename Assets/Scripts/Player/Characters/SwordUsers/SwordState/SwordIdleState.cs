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
        GetSwordCharactersState().GetSwordCharacters().LaunchBasicAttack();
    }

}
