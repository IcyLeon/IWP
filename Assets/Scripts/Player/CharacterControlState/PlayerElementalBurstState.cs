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

    public override void OnAnimationTransition()
    {
        GetPlayerCharacterState().GetPlayerCharacters().SetBurstActive(false);
        GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().GetPlayerElementalSkillandBurstManager().SubscribeBurstState(GetPlayerCharacterState().GetPlayerCharacters());
    }
}
