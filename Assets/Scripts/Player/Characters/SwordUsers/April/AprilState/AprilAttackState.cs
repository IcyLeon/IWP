using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilAttackState : AprilIdleState
{
    public override void Enter()
    {
        base.Enter();

        GetAprilState().GetApril().LookAtClosestTarget();
        GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase++;
        string AtkName = "Attack" + GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase;
        StartAnimation(AtkName);
    }

    public override void Update()
    {
        base.Update();

        if (!GetAprilState().GetApril().GetPlayerManager().IsAttacking())
        {
            GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordIdleState);
            return;
        }
    }
    public AprilAttackState(PlayerCharacterState pcs) : base(pcs)
    {
    }
}
