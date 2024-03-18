using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControlState : PlayerControlState
{
    public SwordControlState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetPlayerCharacterState().GetPlayerCharacters().ToggleOffAimCameraDelay(0);
    }                                 
}
