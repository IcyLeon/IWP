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
        base.OnAnimationTransition();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
