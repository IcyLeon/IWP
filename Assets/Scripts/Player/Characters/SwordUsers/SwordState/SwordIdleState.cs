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
    }
    public override void ChargeTrigger()
    {
        if (GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDashing() || GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsDeadState())
            return;

        LaunchBasicAttack();
    }

    private void UpdateJumpState()
    {
        if (!GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsJumping())
            return;

        GetSwordCharactersState().ChangeState(GetSwordCharactersState().swordJumpState);
    }

    public override void Update()
    {
        base.Update();
        UpdateJumpState();
    }

    protected override void LaunchBasicAttack()
    {
        if (Time.timeScale == 0 || HasReachedEndOfBasicAttackAnimation())
            return;

        if (Time.time - GetPlayerCharacterState().CommonCharactersData.LastClickedTime > CommonCharactersData.AttackRate && !GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().IsSkillCasting())
        {
            TransitionToAttackState();
            GetPlayerCharacterState().CommonCharactersData.LastClickedTime = Time.time;
            return;
        }
    }
}
