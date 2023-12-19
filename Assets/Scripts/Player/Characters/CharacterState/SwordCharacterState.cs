using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCharacterState : PlayerCharacterState
{
    public SwordIdleState swordIdleState;

    public SwordCharacters GetSwordCharacters()
    {
        return (SwordCharacters)GetPlayerCharacters();
    }
    public SwordCharacterState(Characters Characters) : base(Characters)
    {
        
    }
}
