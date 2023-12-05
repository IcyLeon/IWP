using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerMovementState
{
    public PlayerAimState(PlayerState playerState) : base(playerState)
    {
    }
    public override void Enter()
    {
        base.Enter();
        SetSpeedModifier(0.5f);
    }

    public override void FixedUpdate()
    {
        Float();

        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }

        if (GetPlayerState().GetPlayerController().GetCharacterManager().GetCurrentCharacter() is Kaqing)
        {
            return;
        }

        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        if (!IsAiming())
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        ResetSpeedModifier();
    }

}
