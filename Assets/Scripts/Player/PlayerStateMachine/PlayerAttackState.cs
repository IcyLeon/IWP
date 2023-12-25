using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerGroundState
{
    private PlayerCharacters playerCharacter;
    public PlayerAttackState(PlayerState playerState) : base(playerState)
    {
    }
    public override void Enter()
    {
        base.Enter();
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

        
        playerCharacter = GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter();
        if (playerCharacter != null)
        {
            if (!playerCharacter.GetisAttacking() && GetPlayerState().GetPlayerMovementState() is not PlayerDashState || GetPlayerState().GetPlayerController().GetPlayerManager().IsSkillCasting())
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
