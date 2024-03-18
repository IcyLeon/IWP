using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCharacterState : PlayerCharacterState
{
    public SwordIdleState swordIdleState;
    public SwordJumpState swordJumpState;

    public SwordCharacters GetSwordCharacters()
    {
        return (SwordCharacters)GetPlayerCharacters();
    }
    public SwordCharacterState(Characters Characters) : base(Characters)
    {
        characterDeadState = new SwordDeadState(this);
        swordJumpState = new SwordJumpState(this);
    }
}
