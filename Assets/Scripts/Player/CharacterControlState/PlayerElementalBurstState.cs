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
        GetPlayerCharacterState().GetPlayerCharacters().IBurstEnter();
    }

    public override void OnAnimationTransition()
    {
        GetPlayerCharacterState().GetPlayerCharacters().SetBurstActive(false);
    }

    public override void Exit()
    {
        base.Exit();
        GetPlayerCharacterState().GetPlayerCharacters().IBurstExit();
    }
}
