using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControlState : PlayerControlState
{
    public SwordCharacterState GetSwordCharactersState()
    {
        return (SwordCharacterState)GetPlayerCharacterState();
    }

    public SwordControlState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
