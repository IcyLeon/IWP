using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerMovementState
{
    private bool StartDead;
    public PlayerDeadState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation("isDead");
        StartDead = false;
    }

    public override void Exit()
    {
        base.Exit();
        ChangeCharacter();
    }

    public override void FixedUpdate()
    {
        LimitFallVelocity();
        Float();
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }

    private void ChangeCharacter()
    {
        GetPlayerState().GetPlayerController().GetPlayerManager().SwapCharacters(GetAliveCharacters(), true);
    }

    private CharacterData GetAliveCharacters()
    {
        return GetPlayerState().GetPlayerController().GetPlayerManager().GetAliveCharacters();
    }

    public override void Update()
    {
        base.Update();

        if (StartDead && IsTouchingTerrain())
        {
            if (GetAliveCharacters() != null)
            {
                GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
                return;
            }
            else
            {
                GetPlayerState().GetPlayerController().GetMainUI().OpenFallenPanel();
                return;
            }
        }
    }

    public override void OnAnimationTransition()
    {
        StartDead = true;
    }
}
