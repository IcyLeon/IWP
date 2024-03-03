using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordIdleState : SwordControlState
{
    public SwordIdleState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetPlayerCharacterState().GetPlayerCharacters().UpdateDefaultPosOffsetAndZoom(0);
    }
    public override void ChargeTrigger()
    {
        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing() || GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().isDeadState())
            return;

        LaunchBasicAttack();
    }

    protected override void LaunchBasicAttack()
    {
        if (Time.timeScale == 0 || HasReachedEndOfBasicAttackAnimation())
            return;

        if (Time.time - GetPlayerCharacterState().CommonCharactersData.LastClickedTime > CommonCharactersData.AttackRate && !GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsSkillCasting())
        {
            GetPlayerCharacterState().GetPlayerCharacters().LookAtClosestTarget();
            GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase++;

            string AtkName = "Attack" + GetPlayerCharacterState().CommonCharactersData.BasicAttackPhase;
            Animator animator = GetPlayerCharacterState().GetPlayerCharacters().GetAnimator();
            if (Characters.ContainsParam(animator, AtkName))
                animator.SetBool(AtkName, true);

            GetPlayerCharacterState().CommonCharactersData.LastClickedTime = Time.time;
        }
    }
}
