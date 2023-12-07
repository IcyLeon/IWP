using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerMovementState
{
    private float DeathPlayTimer = 1f;
    private float DeathElapsed;

    public PlayerDeadState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerStateEnum = PlayerStateEnum.DEAD;
    }

    public override void Exit()
    {
        base.Exit();
        ChangeCharacter();
        DeathElapsed = 0f;
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
        GetPlayerState().GetPlayerController().GetCharacterManager().SwapCharacters(GetAliveCharacters());
    }

    private CharacterData GetAliveCharacters()
    {
        return GetPlayerState().GetPlayerController().GetCharacterManager().GetAliveCharacters();
    }

    public override void Update()
    {
        base.Update();

        if (DeathElapsed > DeathPlayTimer && IsTouchingTerrain())
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
        DeathElapsed += Time.deltaTime;
    }
}
