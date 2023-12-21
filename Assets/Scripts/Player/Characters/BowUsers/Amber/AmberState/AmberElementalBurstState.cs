using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberElementalBurstState : BowElementalBurstState
{
    public AmberState GetAmberState()
    {
        return (AmberState)GetBowCharactersState();
    }

    public AmberElementalBurstState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void OnAnimationTransition()
    {
        GetAmberState().GetAmber().SpawnAura();
        GetAmberState().GetPlayerCharacters().GetPlayerManager().GetPlayerController().GetPlayerCoordinateAttackManager().Subscribe(GetAmberState().GetAmber());
        base.OnAnimationTransition();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
