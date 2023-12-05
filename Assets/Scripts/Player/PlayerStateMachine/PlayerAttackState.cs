using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerMovementState
{
    private bool IsAttacking;
    public PlayerAttackState(PlayerState playerState) : base(playerState)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void FixedUpdate()
    {
        if (IsMovingHorizontally())
        {
            DecelerateHorizontal();
        }
    }

    public override void Update()
    {
        base.Update();
        if (!IsAttacking)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
        } 
    }

    public override void Exit()
    {
        base.Exit();
    }

}
