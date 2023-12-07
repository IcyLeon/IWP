using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerMovementState
{
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
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }


    public override void Update()
    {
        base.Update();

        if (IsAiming())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerAimState);
            return;
        }

        PlayerCharacters playerCharacter = GetPlayerState().GetPlayerController().GetCharacterManager().GetCurrentCharacter();
        if (playerCharacter != null)
        {
            if (!playerCharacter.GetisAttacking())
            {
                GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
                return;
            }
        }

    }

    public override void Exit()
    {
        base.Exit();
    }

}
