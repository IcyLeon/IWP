using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDeadState : PlayerControlState
{
    public override void Enter()
    {
        base.Enter();
        GetPlayerCharacterState().GetPlayerCharacters().PlayRandomFallenVoice();
    }

    public CharacterDeadState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
