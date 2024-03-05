using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerMovementState
{
    public PlayerAirborneState(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        PlayerController.OnChargeTrigger += OnPlungeAttack;
    }

    private void OnPlungeAttack()
    {
        float PlungeAttackRange = 2f;

        if (GetPlayerState().GetPlayerManager().GetCurrentCharacter() == null)
            return;

        if (!Physics.Raycast(GetPlayerState().rb.position, Vector3.down, PlungeAttackRange, ~LayerMask.GetMask("Ignore Raycast", "Ignore Collision"), QueryTriggerInteraction.Ignore) && !IsTouchingTerrain())
        {
            if (this is not PlayerPlungeState && !GetPlayerState().GetPlayerManager().IsSkillCasting())
            {
                GetPlayerState().ChangeState(GetPlayerState().playerPlungeState);
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        PlayerController.OnChargeTrigger -= OnPlungeAttack;
    }
}
