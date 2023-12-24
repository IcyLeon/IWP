using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerMovingState : PlayerGroundState
{
    public PlayerMovingState(PlayerState playerState) : base(playerState)
    {
    }

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
    }
}
