using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ResetSpeed();
        GetPlayerState().PlayerData.CurrentJumpForceXZ = 0f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        PlayerCharacters playerCharacter = GetPlayerState().GetPlayerController().GetPlayerManager().GetCurrentCharacter();
        if (playerCharacter != null)
        {
            if (playerCharacter.GetisAttacking())
            {
                GetPlayerState().ChangeState(GetPlayerState().playerAttackState);
                return;
            }
        }

        if (GetInputDirection() != Vector3.zero && GetPlayerState().GetPlayerMovementState() is not PlayerJumpState)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerRunState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
