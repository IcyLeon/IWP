using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerGroundState
{
    public PlayerAttackState(PlayerState playerState) : base(playerState)
    {
    }

    public override void FixedUpdate()
    {
        Float();

        if (!IsMovingHorizontally())
        {
            return;
        }

        DecelerateHorizontal();
    }


    public override void Update()
    {
        base.Update();

        
        playerCharacter = GetPlayerState().GetPlayerManager().GetCurrentCharacter();
        if (playerCharacter != null)
        {
            if ((!playerCharacter.GetisAttacking() && GetPlayerState().GetPlayerMovementState() is not PlayerDashState) || GetPlayerState().GetPlayerManager().IsSkillCasting())
            {
                GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
                return;
            }
        }

    }

    public override void Exit()
    {
        base.Exit();

        if (playerCharacter != null)
        {
            //if (playerCharacter.GetisAttacking())
            //    playerCharacter.ResetAttack();
        }
    }

}
