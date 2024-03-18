using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingAttackState : KaqingIdleState
{
    public override void Enter()
    {
        base.Enter();

        Kaqing.LookAtClosestTarget();
        GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase++;
        string AtkName = "Attack" + GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase;
        StartAnimation(AtkName);
    }

    public override void Update()
    {
        base.Update();

        if (!Kaqing.GetPlayerManager().IsAttacking())
        {
            GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordIdleState);
            return;
        }
    }

    public KaqingAttackState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
