using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerMovementState
{
    private bool canFloat;
    private float FloatTimeElapsed;

    public bool GetCanFloat()
    {
        return canFloat;
    }
    protected void StayAfloatFor(float sec)
    {
        float time = sec;
        canFloat = true;
        if (time <= 0)
            time = 0f;

        FloatTimeElapsed = time;
        ResetVelocity();
        rb.useGravity = false;
    }

    public PlayerAirborneState(PlayerState playerState) : base(playerState)
    {
        canFloat = false;
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
        FloatAbove();
        CheckPlunge();
    }

    protected virtual void FloatAbove()
    {
        if (FloatTimeElapsed > 0)
        {
            if (canFloat)
            {
                FloatTimeElapsed -= Time.deltaTime;
            }
        }
        else
        {
            canFloat = false;
            rb.useGravity = true;
        }
    }
    private void CheckPlunge()
    {
        float PlungeAttackRange = 2f;
        int layerMask = Physics.DefaultRaycastLayers;

        if (!Physics.Raycast(rb.position, Vector3.down, PlungeAttackRange, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (Input.GetMouseButtonDown(0) && GetPlayerState().GetPlayerMovementState() is not PlayerPlungeState)
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
