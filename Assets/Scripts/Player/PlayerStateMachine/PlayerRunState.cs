using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerMovingState
{
    public PlayerRunState(PlayerState playerState) : base(playerState)
    {
    }

    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
        ResetSpeed();
        GetPlayerState().PlayerData.CurrentJumpForceXZ = 3.5f;
    }

    public override float GetAnimationSpeed()
    {
        return 0.5f;
    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (GetPlayerState().PlayerData.Direction == Vector3.zero)
        {
            GetPlayerState().ChangeState(GetPlayerState().playerIdleState);
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
        ResetSpeed();
    }
}
