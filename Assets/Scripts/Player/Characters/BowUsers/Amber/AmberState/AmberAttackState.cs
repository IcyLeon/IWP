using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberAttackState : AmberIdleState
{
    public override void Enter()
    {
        base.Enter();

        GetBowData().Direction = GetBowCharactersState().GetPlayerCharacters().LookAtClosestTarget();
        GetAmberState().GetAmber().LookAtClosestTarget();
        GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase = 1;
        string AtkName = "Attack" + GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase;
        StartAnimation(AtkName);
    }

    public override void Update()
    {
        base.Update();

        if (!GetAmberState().GetAmber().GetPlayerManager().IsAttacking())
        {
            GetBowCharactersState().ChangeState(GetBowCharactersState().bowIdleState);
            return;
        }
    }

    public AmberAttackState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
