using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElementalBurstState : PlayerControlState
{
    public PlayerElementalBurstState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();

        GetPlayerCharacterState().GetPlayerCharacters().PlayRandomSkillsBurstVoice();
    }
}
