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
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        CheckPlunge();
    }

    private void CheckPlunge()
    {
        float PlungeAttackRange = 2f;

        if (!Physics.Raycast(GetPlayerState().rb.position, Vector3.down, PlungeAttackRange, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore) && !IsTouchingTerrain())
        {
            if (Input.GetMouseButtonDown(0) && this is not PlayerPlungeState && !GetPlayerState().GetPlayerController().GetPlayerManager().IsSkillCasting())
            {
                GetPlayerState().ChangeState(GetPlayerState().playerPlungeState);
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
